using System;
using Configuration.Exception;
using Controller;
using UnityEngine;
using Util;

public class CameraController : MonoBehaviour
{
    private float _maxCameraOrthographicSize;
    private float _minCameraOrthographicSize;
    private float _initialXPosition;
    private float _maxDeltaX;
    private float _previousDistance = CameraConstantsUtil.ZeroFloatValue;
    private Camera _camera;
    private Vector2 _firstTouchPosition = Vector2.zero;
    private BootController _bootController;
    private void Start()
    {
        _camera = Camera.main;
        if (_camera == null)
            throw new CustomMissingComponentException(TagUtil.MainCamera);
        GetBootGameObject();
        var position = _camera.transform.position;
        _initialXPosition = position.x;
        var orthographicSize = _camera.orthographicSize;
        _maxCameraOrthographicSize = orthographicSize;
        _minCameraOrthographicSize = _maxCameraOrthographicSize - CameraConstantsUtil.MaxDeltaOrthographicSize;
        _maxDeltaX = CalculateCameraXBorders(orthographicSize);
    }

    private void GetBootGameObject()
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
            var firstTouch = Input.touches[0];
            var secondTouch = Input.touches[1];
            var distance = Vector2.Distance(firstTouch.position, secondTouch.position);
            if (Math.Abs(_previousDistance - CameraConstantsUtil.ZeroFloatValue) < 0.0001f)
            {
                _previousDistance = distance;
            }
            else
            {
                ScaleCamera(distance);
            }
        }
        else
        {
            if (Input.touchCount == 1 && !_bootController.IsDragging)
            {
                MoveCamera(Input.touches[0]);
            }

            _previousDistance = CameraConstantsUtil.ZeroFloatValue;
        }
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
        var worldTouchPosition = _camera.ScreenToWorldPoint(touchPosition);
        var worldFirstTouchPosition = _camera.ScreenToWorldPoint(_firstTouchPosition);
        var resultVector = -(worldTouchPosition - worldFirstTouchPosition);
        var newTransformVector = new Vector3(resultVector.x, resultVector.y, 0);
        transform.position = CorrectDeltaPosition(newTransformVector);
    }

    private Vector3 CorrectDeltaPosition(Vector3 newTransformVector)
    {
        var position = transform.position;
        var currentDeltaX = newTransformVector.x * Time.deltaTime * CameraConstantsUtil.DeltaTimeCoefficient;
        var currentDeltaY = newTransformVector.y * Time.deltaTime * CameraConstantsUtil.DeltaTimeCoefficient;
        var newX = Mathf.Clamp(position.x + currentDeltaX, _initialXPosition - _maxDeltaX, 
            _initialXPosition + _maxDeltaX);
        if (Math.Abs(_initialXPosition - _maxDeltaX - newX) < 0.001f || Math.Abs(_initialXPosition + _maxDeltaX - newX) < 0.001f)
        {
            currentDeltaX = 0f;
        }
        //todo: zoom somehow works, need to set moving borders
        return position + new Vector3(currentDeltaX, 0f, 0f);
    }

    private void ScaleCamera(float distance)
    {
        if (_previousDistance < distance)
        {
            _camera.orthographicSize =  _camera.orthographicSize - Time.deltaTime * distance / CameraConstantsUtil.DistanceCoefficient;
        }
        else if (_previousDistance > distance)
            
        {
            var newOrthographicSize = _camera.orthographicSize + Time.deltaTime * distance / CameraConstantsUtil.DistanceCoefficient;
            if (Math.Abs(_camera.orthographicSize - _maxCameraOrthographicSize) > 0.001f)
            {
                MoveBeforeZoomIfNeeded(newOrthographicSize);   
            }
            _camera.orthographicSize = newOrthographicSize;
        }

        CorrectScale();
    }

    private void MoveBeforeZoomIfNeeded(float orthographicSize)
    {
        var position = _camera.transform.position;
        var deltaX = CalculateCameraXBorders(orthographicSize) * Time.deltaTime * CameraConstantsUtil.DeltaTimeCoefficient;
        if (position.x + deltaX > _initialXPosition + _maxDeltaX)
        {
            _camera.transform.position = new Vector3(position.x - deltaX, position.y, position.z);
        } else if (position.x - deltaX < _initialXPosition - _maxDeltaX)
        {
            _camera.transform.position = new Vector3(position.x + deltaX, position.y, position.z);
        }
    }
    
    private float CalculateCameraXBorders(float orthographicSize)
    {
        var screenAspect = Screen.width / (float) Screen.height;
        var camHalfWidth = screenAspect * orthographicSize;
        return _initialXPosition + camHalfWidth;
    }

    private void CorrectScale()
    {
        _camera.orthographicSize =
            Mathf.Clamp(_camera.orthographicSize, _minCameraOrthographicSize, _maxCameraOrthographicSize);
    }
}