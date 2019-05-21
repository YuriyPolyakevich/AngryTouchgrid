using System;
using UnityEngine;

namespace Controller
{
    public class BootController : MonoBehaviour
    {
        private bool _isDragging = false;
        private Vector3 _firstPosition = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Rigidbody _rigidBody = null;
        private const int Force = 2000;
        private Camera _camera;
        private GoalController _goalController;
        private void Start()
        {
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
                MoveObject();
                _isDragging = false;
            }
        }

        private bool IsHitted(Touch touch)
        {
            RaycastHit raycastHit;
            var ray = _camera.ScreenPointToRay(touch.position);
            return Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.tag.Equals("Player");;
        }

        private void MoveObject()
        {
            var moveDirection = -(_lastPosition - _firstPosition);
            _rigidBody.AddForce(moveDirection * Force, ForceMode.Force);
            _goalController.BootKickedTime = DateTime.Now;
        }

        private void StopDraggingConfiguration(Touch touch)
        {
            _lastPosition = touch.position;
        }

        private void StartDraggingConfiguration(Touch touch)
        {
            if (!IsHitted(touch)) return;
            _isDragging = true;
            _firstPosition = touch.position;
        }
    }
}