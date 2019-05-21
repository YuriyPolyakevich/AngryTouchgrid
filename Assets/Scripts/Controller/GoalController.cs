using System;
using System.Collections;
using a;
using UnityEngine;
using Util;

namespace Controller
{
    public class GoalController : MonoBehaviour
    {
        private bool _isGoal = false;
        private bool _isFinish = false;
        private bool _noGoal = false;
        public bool IsBallKicked { private get; set; }
        public DateTime BootKickedTime { get; set; }
        private const float ToleranceDistance = 0.0009f;
        private const float ToleranceVelocity = 0.0005f;
        private float _previousBallDistance;
        private float _previousBootDistance;
        private GUIStyle _guiStyle;
        private Rigidbody _ballRigidBody;
        private Rigidbody _playerRigidBody;
        private Rect _rect;
        private Vector3 _goalLinePosition;

        private void Start()
        {
            BootKickedTime = DateTime.MinValue;
            _rect = new Rect(x: Screen.width / 2, y: Screen.height / 2, width: Screen.width / 6,
                height: Screen.height / 4);
            _guiStyle = new GUIStyle {fontSize = 40};
            _ballRigidBody = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();
            _playerRigidBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
            _goalLinePosition = transform.position;
            _previousBallDistance = Vector3.Distance(_ballRigidBody.position, _goalLinePosition);
            _previousBootDistance = Vector3.Distance(_ballRigidBody.position, _playerRigidBody.position);
        }

        private void Update()
        {
            if (BootKickedTime != DateTime.MinValue && !IsBallKicked)
            {
                var timeDifference = DateTime.Now - BootKickedTime;
                var milliseconds = (int)timeDifference.TotalMilliseconds;
                if (milliseconds > 3000 && !IsBootBallDistanceDecreasing(_playerRigidBody.position))
                {
                    _noGoal = true;   
                }
            }
            if (!IsBallKicked || _isGoal) return;
            if (!IsDistanceDecreasing(_ballRigidBody.transform.position))
            {
                _noGoal = true;
            }
            else if (_ballRigidBody.velocity.x < ToleranceVelocity
                     && _ballRigidBody.velocity.y < ToleranceVelocity
                     && _ballRigidBody.velocity.z < ToleranceVelocity
                     && Vector3.Distance(_ballRigidBody.transform.position, _goalLinePosition) > ToleranceDistance)
            {
                _noGoal = true;
            }
        }

        private bool IsBootBallDistanceDecreasing(Vector3 position)
        {
            var currentDistance = Vector3.Distance(position, _ballRigidBody.position);
            if (!(currentDistance - _previousBootDistance + ToleranceDistance < 0)) return false;
            _previousBootDistance = currentDistance;
            return true;
        }

        private bool IsDistanceDecreasing(Vector3 ballPosition)
        {
            var currentDistance = Vector3.Distance(ballPosition, _goalLinePosition);
            if (!(currentDistance - _previousBallDistance + ToleranceDistance < 0)) return false;
            _previousBallDistance = currentDistance;
            return true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null || !other.gameObject.tag.Equals("Ball")) return;
            _isGoal = true;
            GoToNextLevel();
        }

        private void OnGUI()
        {
            if (_isGoal)
            {
                GUI.Label(_rect, "GOAL!", _guiStyle);
            }

            if (_isFinish)
            {
                GUI.Label(_rect, "Oh yeah zaebok!", _guiStyle);
            }

            if (_noGoal)
            {
                GUI.Label(_rect, "Proeb", _guiStyle);
            }
        }

        private void GoToNextLevel()
        {
            StartCoroutine(NextLevel());
        }

        private IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(2);
            try
            {
                LevelUtil.LoadNextLevel();
            }
            catch (Exception e)
            {
                if (GlobalConfiguration.IsDevMode())
                {
                    Debug.unityLogger.Log(e);
                }

                _isGoal = false;
                _isFinish = true;
            }
        }
    }
}