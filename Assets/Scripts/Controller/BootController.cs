using System;
using System.Collections;
using System.Collections.Generic;
using Configuration.Exception;
using UnityEngine;
using Util;

namespace Controller
{
    public class BootController : MonoBehaviour
    {
        public bool IsBootMoving { private set; get; }
        public bool IsDragging { get; private set; }
        private Vector3 _initialPosition;
        private Vector3 _firstPosition = Vector3.zero;
        private GameObject _slingShot;
        private Vector3 _slingShotPosition = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Rigidbody _rigidBody;
        private Camera _camera;
        private GoalController _goalController;
        private Vector3 _forceVector = Vector3.zero;
        private DateTime _bootKickedTime = DateTime.MinValue;
        private GameManagerUtil _gameManagerUtil;
        private bool _isLastBoot;
        private readonly List<GameObject> _dotPrefabs = new List<GameObject>();

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            if (_rigidBody == null)
            {
                throw new CustomMissingComponentException(TagUtil.RigidBody);
            }
            _initialPosition = transform.position;
            _rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
            GetGameObjects();
        }

        private void GetGameObjects()
        {
            GetCamera();
            GetGoalController();
            GetGameManagerUtil();
            GetBootController();
            GetSlingShotController();
        }

        private void GetSlingShotController()
        {
            _slingShot = GameObject.FindGameObjectWithTag(TagUtil.SlingShot);
            
            if (_slingShot == null)
            {
                throw new MissingTagException(TagUtil.SlingShot);
            }

            _slingShotPosition = _slingShot.transform.position;

            
            if (_slingShot.GetComponent<SlingShotController>() == null)
            {
                throw new CustomMissingComponentException(TagUtil.SlingShotController);
            }
            
            var slingShotController = _slingShot.GetComponent<SlingShotController>();
            slingShotController.BootController = GetComponent<BootController>();
            slingShotController.Boot = gameObject;
        }

        private void GetBootController()
        {
            if (GetComponent<BootController>() == null)
            {
                throw new CustomMissingComponentException(TagUtil.BootController);
            }   
        }

        private void GetGameManagerUtil()
        {
            if (GameObject.FindGameObjectWithTag(TagUtil.GameController) == null)
            {
                throw new MissingTagException(TagUtil.GameController);
            }
            _gameManagerUtil = GameObject.FindGameObjectWithTag(TagUtil.GameController).GetComponent<GameManagerUtil>();
            
            if (_gameManagerUtil == null)
            {
                throw new CustomMissingComponentException(TagUtil.GameManagerUtil);
            }
        }

        private void GetGoalController()
        {
            if (GameObject.FindGameObjectWithTag(TagUtil.GoalController) == null)
            {
                throw new MissingTagException(TagUtil.GoalController);
            }
            
            _goalController = GameObject.FindGameObjectWithTag(TagUtil.GoalController).GetComponent<GoalController>();

            if (_goalController == null)
            {
                throw new CustomMissingComponentException(TagUtil.GoalController);
            }

        }

        private void GetCamera()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                throw new MissingTagException(TagUtil.MainCamera);
            }
        }

        private void Update()
        {
            if (_bootKickedTime == DateTime.MinValue)
            {
                if (Input.touches.Length == 1)
                {
                    _firstPosition = _camera.WorldToScreenPoint(_slingShotPosition);
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
            if (!IsDragging) return;

            if (CorrectDirection())
            {
                MoveObject();
                ClearDots();
                StartCoroutine(InstantiateNewBoot());
            }
            else
            {
                transform.position = _initialPosition;
            }

            IsDragging = false;
        }

        private IEnumerator InstantiateNewBoot()
        {
            yield return new WaitForSeconds(1.5f);
            _isLastBoot = _goalController.DecreaseLife();
        }

        private void ShowTrajectory()
        {
            var touch = Input.touches[0];
            if (!IsDragging)
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
            if (!IsBootHit(touch)) return;
            IsDragging = true;
            InstantiateDots();
        }

        private void InstantiateDots()
        {
            for (var i = 0; i < BootConstantsUtil.NumOfDotsToShow; i++)
            {
                var trajectoryDot = Instantiate(_gameManagerUtil.dotPrefab);
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

        private bool IsBootHit(Touch touch)
        {
            RaycastHit rayCastHit;
            var ray = _camera.ScreenPointToRay(touch.position);
            return Physics.Raycast(ray, out rayCastHit) && rayCastHit.transform.gameObject.tag.Equals(TagUtil.Player);
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
            ChangeBootPosition(touch.position);
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

        private void ChangeBootPosition(Vector2 touchPosition)
        {
            var screenPointToWorld = _camera.ScreenToWorldPoint(touchPosition);
            var xPosition = Mathf.Clamp(screenPointToWorld.x, _initialPosition.x - BootConstantsUtil.XDistanceConstraint,
                _initialPosition.x + BootConstantsUtil.XDistanceConstraint);
            var yPosition = Mathf.Clamp(screenPointToWorld.y, _initialPosition.y - BootConstantsUtil.YDownDistanceConstraint,
                _initialPosition.y + BootConstantsUtil.YUpDistanceConstraint);
            var position = new Vector3(xPosition, yPosition, BootConstantsUtil.ZFreezePosition);
            transform.position = position;
            _lastPosition = _camera.WorldToScreenPoint(position);
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
            var calculatedPosition = transform.position +
                                    (_forceVector / mass) * elapsedTime
                                    + BootConstantsUtil.GravityVector * elapsedTime * elapsedTime / 2;

            return calculatedPosition;
        }
    }
}