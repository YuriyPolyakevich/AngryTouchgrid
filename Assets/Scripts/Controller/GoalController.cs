using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private const float ToleranceDistance = 0.9f;
        private const float ToleranceVelocity = 0.0005f;
        private float _previousBallDistance;
        private float _previousBootDistance;
        private GUIStyle _guiStyle;
        private Rigidbody _ballRigidBody;
        private Rigidbody _playerRigidBody;
        private Rect _rect;
        private Vector3 _goalLinePosition;
        private static int _lifes = 3;
        private static int _previousSceneBuildIndex;

        private bool _isLifeWasDecremented;

        private void Start()
        {
            if (_previousSceneBuildIndex != SceneManager.GetActiveScene().buildIndex)
            {
                _lifes = 3;
                _previousSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            }

            _isLifeWasDecremented = false;
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
            if (_isFinish) return;
            if (_isGoal) return;
            if (BootKickedTime != DateTime.MinValue && !IsBallKicked)
            {
                var timeDifference = DateTime.Now - BootKickedTime;
                var milliseconds = (int) timeDifference.TotalMilliseconds;
                if (milliseconds > 3000 && !IsBootBallDistanceDecreasing(_playerRigidBody.position))
                {
                    _noGoal = true;
                }
            }

            if (IsBallKicked && !_isGoal)
            {
                if (!IsBallGoalDistanceDecreasing(_ballRigidBody.transform.position))
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

            if (!_noGoal || _lifes <= 0 || _isLifeWasDecremented) return;
            Debug.unityLogger.Log(_noGoal + " : " + _isLifeWasDecremented+ " : " +_lifes);
            _lifes--;
            _isLifeWasDecremented = true;
            StartCoroutine(ReturnObjectsToStartPositions());
        }

        private void UpdateScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator ReturnObjectsToStartPositions()
        {
            yield return new WaitForSeconds(2);
            UpdateScene();
        }

        private bool IsBootBallDistanceDecreasing(Vector3 position)
        {
            var currentDistance = Vector3.Distance(position, _ballRigidBody.position);
            if (!(currentDistance - _previousBootDistance + ToleranceDistance < 0)) return false;
            _previousBootDistance = currentDistance;
            return true;
        }

        private bool IsBallGoalDistanceDecreasing(Vector3 ballPosition)
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

            if (_noGoal && _lifes == 0)
            {
                GUI.Label(_rect, "Proeb", _guiStyle);
            }

            if (!_isLifeWasDecremented) return;
            GUI.Label(_rect, "Remainig " + _lifes + " lifes", _guiStyle);
        }

        private void GoToNextLevel()
        {
            StartCoroutine(NextLevel());
        }

        private IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(2);
            if (LevelUtil.LoadNextLevel()) yield break;
            _isLifeWasDecremented = false;
            _isGoal = false;
            _isFinish = true;
        }
    }
}