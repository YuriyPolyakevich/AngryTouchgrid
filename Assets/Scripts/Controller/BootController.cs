using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Controller
{
    public class BootController : MonoBehaviour
    {
        public GameObject trajectoryDotPrefab;
        public bool IsBootMoving { private set; get; }
        private Vector3 _initialPosition;
        private bool _isDragging = false;
        private Vector3 _firstPosition = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Rigidbody _rigidBody = null;
        private Camera _camera;
        private GoalController _goalController;
        private readonly List<GameObject> _dotPrefabs = new List<GameObject>();
        private Vector3 _forceVector = Vector3.zero;

        private void Start()
        {
            _initialPosition = transform.position;
            _rigidBody = gameObject.GetComponent<Rigidbody>();
            _goalController = GameObject.FindGameObjectWithTag("GoalController").GetComponent<GoalController>();
            _rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_goalController.BootKickedTime != DateTime.MinValue) return;
            if (Input.touches.Length == 1)
            {
                var touch = Input.touches[0];
                if (!_isDragging)
                {
                    StartDraggingConfiguration(touch);
                }
                else
                {
                    DraggingConfiguration(touch);
                }
            }
            else
            {
                if (!_isDragging) return;
                
                if (CorrectDirection())
                {
                    MoveObject();
                    ClearDots();
                }
                else
                {
                    transform.position = _initialPosition;
                }
                _isDragging = false;
            }
        }

        private bool CorrectDirection()
        {
            return CalculateForce().x > _initialPosition.x;
        }

        private void StartDraggingConfiguration(Touch touch)
        {
            if (!IsHit(touch)) return;
            _isDragging = true;
            _firstPosition = touch.position;
            InstantiateDots();
        }

        private void InstantiateDots()
        {
            for (var i = 0; i < BootConstantsUtil.NumOfDotsToShow; i++)
            {
                var trajectoryDot = Instantiate(trajectoryDotPrefab);
                _dotPrefabs.Add(trajectoryDot);
            }
        }

        public void FixedUpdate()
        {
            if (IsBootMoving)
            {
                _rigidBody.AddForce(BootConstantsUtil.GravityVector, ForceMode.Acceleration);
            }
        }

        private bool IsHit(Touch touch)
        {
            RaycastHit rayCastHit;
            var ray = _camera.ScreenPointToRay(touch.position);
            return Physics.Raycast(ray, out rayCastHit) && rayCastHit.transform.gameObject.tag.Equals("Player");
        }

        private void MoveObject()
        {
            if (_forceVector == Vector3.zero) return;
            IsBootMoving = true;
            _rigidBody.AddForce(_forceVector, ForceMode.Impulse);
            _goalController.BootKickedTime = DateTime.Now;
        }

        private Vector3 CalculateForce()
        {
            return -(_lastPosition - _firstPosition) * BootConstantsUtil.Force;
        }

        private void DraggingConfiguration(Touch touch)
        {
            _lastPosition = touch.position;
            ChangeBootPosition();
            if (CorrectDirection())
            {
                if (_dotPrefabs.Count == 0)
                    InstantiateDots();
                for (var i = 0; i < _dotPrefabs.Count; i++)
                {
                    _dotPrefabs[i].transform.position = CalculateDotPosition(BootConstantsUtil.DotTimeStep * (i + 1));
                }   
            }
            else
            {
                ClearDots();
            }
        }

        private void ChangeBootPosition()
        {
            var screenPointToWorld = _camera.ScreenToWorldPoint(_lastPosition);
            var xPosition = screenPointToWorld.x;
            var yPosition = screenPointToWorld.y;
            if (xPosition > _initialPosition.x + BootConstantsUtil.XDistanceConstraint)
            {
                xPosition = _initialPosition.x + BootConstantsUtil.XDistanceConstraint;
            }
            else if (xPosition < _initialPosition.x - BootConstantsUtil.XDistanceConstraint)
            {
                xPosition = _initialPosition.x - BootConstantsUtil.XDistanceConstraint;
            }

            if (yPosition > _initialPosition.y + BootConstantsUtil.YUpDistanceConstraint)
            {
                yPosition = _initialPosition.y + BootConstantsUtil.YUpDistanceConstraint;
            }
            else if (yPosition < _initialPosition.y - BootConstantsUtil.YDownDistanceConstraint)
            {
                yPosition = _initialPosition.y - BootConstantsUtil.YDownDistanceConstraint;
            }
            transform.position = new Vector3(xPosition, yPosition, BootConstantsUtil.ZFreezePosition);
        }

        private void ClearDots()
        {
            foreach (var i in _dotPrefabs)
            {
                Destroy(i);
            }
            _dotPrefabs.Clear();
        }


        private Vector3 CalculateDotPosition(float elapsedTime)
        {
            var mass = _rigidBody.mass;
            _forceVector = CalculateForce();
            var calculatePosition = transform.position +
                                    (_forceVector / mass) * elapsedTime
                                    + BootConstantsUtil.GravityVector * elapsedTime * elapsedTime / 2;

            return calculatePosition;
        }
    }
}