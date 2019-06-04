using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Controller
{
    public class BootController : MonoBehaviour
    {
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
        private DateTime _bootKickedTime = DateTime.MinValue;
        private GameManagerUtil _gameManagerUtil;
        private bool _isLastBoot;

        private void Start()
        {
            _initialPosition = transform.position;
            _rigidBody = gameObject.GetComponent<Rigidbody>();
            _goalController = GameObject.FindGameObjectWithTag("GoalController").GetComponent<GoalController>();
            _gameManagerUtil = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerUtil>();
            _rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_bootKickedTime == DateTime.MinValue)
            {
                if (Input.touches.Length == 1)
                {
                    ShowTrajectory();
                }
                else
                {
                    ThrowBoot();
                }
            }
            else
            {
                if (Mathf.Abs(_rigidBody.velocity.x) < Mathf.Abs(BootConstantsUtil.VelocityToleranceVector.x)
                    && Mathf.Abs(_rigidBody.velocity.y) < Mathf.Abs(BootConstantsUtil.VelocityToleranceVector.y)
                    && Mathf.Abs(_rigidBody.velocity.z) < Mathf.Abs(BootConstantsUtil.VelocityToleranceVector.z))
                {
                    StartCoroutine(DestroyBoot());
                }
            }
        }

        //todo: add destroying animation
        private IEnumerator DestroyBoot()
        {
            yield return new WaitForSeconds(2f);
            if (_isLastBoot)
            {
                _goalController.SetLastBootDestroyed();
            }
            Destroy(gameObject);
        }

        private void ThrowBoot()
        {
            if (!_isDragging) return;

            if (CorrectDirection())
            {
                MoveObject();
                ClearDots();
                StartCoroutine(InstantinateNewBoot());
            }
            else
            {
                transform.position = _initialPosition;
            }

            _isDragging = false;
        }

        private IEnumerator InstantinateNewBoot()
        {
            yield return new WaitForSeconds(1.5f);
            _isLastBoot = _goalController.DecreaseLife();
        }

        private void ShowTrajectory()
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
                var trajectoryDot = Instantiate(_gameManagerUtil.DotPrefab);
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
            _bootKickedTime = DateTime.Now;
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