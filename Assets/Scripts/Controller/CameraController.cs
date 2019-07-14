using Configuration.Exception;
using Controller;
using UnityEngine;
using Util;

//todo: fix bug with boot respawn, fix zoom etc.
public class CameraController : MonoBehaviour
{
    public GameObject CameraLeftBound;
    public GameObject CameraRightBound;
    private float _maxCameraOrthographicSize;
    private float _minCameraOrthographicSize;
    private Camera _camera;
    private Vector2 _firstTouchPosition = Vector2.zero;
    private static BootController _bootController;

    private void Start()
    {
        GetBootGameObject();
        var position = _camera.transform.position;
        var orthographicSize = _camera.orthographicSize;
        _maxCameraOrthographicSize = orthographicSize + CameraConstantsUtil.MaxDeltaOrthographicSize * 2;
        _minCameraOrthographicSize = orthographicSize - CameraConstantsUtil.MaxDeltaOrthographicSize * 2;
    }

    private void GetBootGameObject()
    {
        FindBootGameObject();

        if (GetComponent<Camera>() == null)
        {
            throw new CustomMissingComponentException(TagUtil.MainCamera);
        }

        _camera = GetComponent<Camera>();
    }

    private static void FindBootGameObject()
    {
        var bootGameObject = GameObject.FindGameObjectWithTag(TagUtil.Player);
        if (bootGameObject == null)
        {
            throw new MissingTagException(TagUtil.Player);
        }

        if (bootGameObject.GetComponent<BootController>() == null)
        {
            throw new CustomMissingComponentException(TagUtil.BootController);
        }

        _bootController = bootGameObject.GetComponent<BootController>();
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            ScaleCamera();
        }
        else
        {
            if (Input.touchCount == 1 && !_bootController.IsDragging)
            {
                MoveCamera(Input.touches[0]);
            }

        }
    }

    private void ScaleCamera()
    {
        var touchZero = Input.GetTouch(0);
        var touchOne = Input.GetTouch(1);
        var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
        var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
        var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
        if (!_camera.orthographic) return;
        var orthographicSize = _camera.orthographicSize;
        orthographicSize += deltaMagnitudeDiff * CameraConstantsUtil.OrthographicZoomSpeed;
        _camera.orthographicSize = Mathf.Max(orthographicSize, 0.1f);
        CorrectScale();
    }

    private void MoveCamera(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            _firstTouchPosition = touch.position;
        }
        else
        {
            Move(touch.position);
        }
    }

    private void Move(Vector2 touchPosition)
    {
        var position = transform.position;
        var worldTouchPosition = _camera.ScreenToWorldPoint(touchPosition);
        var worldFirstTouchPosition = _camera.ScreenToWorldPoint(_firstTouchPosition);
        var resultVector = -(worldTouchPosition - worldFirstTouchPosition);
        var newPosition = position + resultVector * Time.deltaTime * CameraConstantsUtil.Speed;
        var xTransform = Mathf.Clamp(newPosition.x, CameraLeftBound.transform.position.x, CameraRightBound.transform.position.x);
        transform.position = new Vector3(xTransform, position.y, position.z);
    }

    private void CorrectScale()
    {
        _camera.orthographicSize =
            Mathf.Clamp(_camera.orthographicSize, _minCameraOrthographicSize, _maxCameraOrthographicSize);
    }

    public static void SetNewBoot(GameObject boot)
    {
        _bootController = boot.GetComponent<BootController>();
    }
}