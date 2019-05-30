using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class BootController : MonoBehaviour
    {
        [FormerlySerializedAs("_trajectoryDotPrefab")]
        public GameObject trajectoryDotPrefab;

        private const float DotTimeStep = 0.05f;
        private bool _isDragging = false;
        private Vector3 _firstPosition = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Rigidbody _rigidBody = null;
        private const int Force = 70;
        private Camera _camera;
        private GoalController _goalController;
        private Vector3 InitialPosition { get; set; }
        private const int NumOfDotsToShow = 10;
        private readonly List<GameObject> _dotPrefabs = new List<GameObject>();
        private readonly Vector3 _gravityVector = new Vector3(0f, -100f, 0);
        private bool _isBootMoving = false;
        private Vector3 _forceVector = Vector3.zero;

        private void Start()
        {
            InitialPosition = transform.position;
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
                    StopDraggingConfiguration(touch);
                }
            }
            else
            {
                if (!_isDragging) return;
                ClearDots();
                MoveObject();
                _isDragging = false;
            }
        }

        private void StartDraggingConfiguration(Touch touch)
        {
            if (!IsHitted(touch)) return;
            _isDragging = true;
            _firstPosition = touch.position;
            InstantiateDots();
        }

        private void InstantiateDots()
        {
            for (var i = 0; i < NumOfDotsToShow; i++)
            {
                var trajectoryDot = Instantiate(trajectoryDotPrefab);
                _dotPrefabs.Add(trajectoryDot);
            }
        }

        public void FixedUpdate()
        {
            if (_isBootMoving)
            {
                _rigidBody.AddForce(_gravityVector, ForceMode.Acceleration);
            }
        }

        private bool IsHitted(Touch touch)
        {
            RaycastHit raycastHit;
            var ray = _camera.ScreenPointToRay(touch.position);
            return Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.tag.Equals("Player");
        }

        private void MoveObject()
        {
            if (_forceVector == Vector3.zero) return;
            _isBootMoving = true;
            _rigidBody.AddForce(_forceVector, ForceMode.Impulse);
            _goalController.BootKickedTime = DateTime.Now;
        }

        private Vector3 CalculateForce()
        {
            return -(_lastPosition - _firstPosition) * Force;
            ;
        }

        private void StopDraggingConfiguration(Touch touch)
        {
            _lastPosition = touch.position;
            for (var i = 0; i < _dotPrefabs.Count; i++)
            {
                _dotPrefabs[i].transform.position = CalculatePosition(DotTimeStep * i);
            }
        }

        private void ClearDots()
        {
            foreach (var i in _dotPrefabs)
            {
                Destroy(i);
            }
        }


        private Vector3 CalculatePosition(float elapsedTime)
        {
            var mass = _rigidBody.mass;
            _forceVector = CalculateForce();
            var calculatePosition = InitialPosition +
                                    (_forceVector / mass) * elapsedTime
                                    + _gravityVector * elapsedTime * elapsedTime / 2;

            return calculatePosition;
        }
    }
}