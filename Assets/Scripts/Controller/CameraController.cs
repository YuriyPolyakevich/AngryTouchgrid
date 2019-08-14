using Configuration.Exception;
using Controller;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

//todo: fix bug with boot respawn, fix zoom etc.
public class CameraController : MonoBehaviour
{
    [FormerlySerializedAs("CameraLeftBound")]
    public GameObject cameraLeftBound;

    [FormerlySerializedAs("CameraRightBound")]
    public GameObject cameraRightBound;

    private float _maxCameraOrthographicSize;
    private float _minCameraOrthographicSize;
    private Camera _camera;
    private Vector2 _firstTouchPosition = Vector2.zero;
    private static BootPerspectiveController _bootController;

    private void Start()
    {
        GetBootGameObject();
        SetCameraOrthographicSizeConstraints();
    }

    private void SetCameraOrthographicSizeConstraints()
    {
        var orthographicSize = _camera.orthographicSize;
        _maxCameraOrthographicSize = orthographicSize + CameraConstantsUtil.MaxDeltaOrthographicSize * 2;
        _minCameraOrthographicSize = orthographicSize - CameraConstantsUtil.MaxDeltaOrthographicSize * 2;
    }

    private void GetBootGameObject()
    {
        FindBootGameObject();
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            throw new CustomMissingComponentException(TagUtil.MainCamera);
        }
    }

    private static void FindBootGameObject()
    {
        var bootGameObject = GameObject.FindGameObjectWithTag(TagUtil.Player);
        if (bootGameObject == null)
        {
            throw new MissingTagException(TagUtil.Player);
        }
        
        _bootController = bootGameObject.GetComponent<BootPerspectiveController>();
        
        if (_bootController == null)
        {
            throw new CustomMissingComponentException(TagUtil.BootController);
        }

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
        var xTransform = Mathf.Clamp(newPosition.x, cameraLeftBound.transform.position.x,
            cameraRightBound.transform.position.x);
        transform.position = new Vector3(xTransform, position.y, position.z);
    }

    private void CorrectScale()
    {
        _camera.orthographicSize =
            Mathf.Clamp(_camera.orthographicSize, _minCameraOrthographicSize, _maxCameraOrthographicSize);
    }

    public static void SetNewBoot(GameObject boot)
    {
        _bootController = boot.GetComponent<BootPerspectiveController>();
    }
}