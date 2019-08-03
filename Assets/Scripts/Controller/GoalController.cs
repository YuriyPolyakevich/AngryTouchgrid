using System;
using System.Collections;
using Configuration;
using Configuration.Exception;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Controller
{
    public class GoalController : MonoBehaviour
    {
        private bool _isGoal = false;
        private bool _isWin = false;
        private bool _isLost = false;
        public GameObject BootPrefab;
        private GUIStyle _guiStyle;
        private Rect _rect;
        private static int _lives;
        private static int _previousSceneBuildIndex;
        private DateTime _noGoalDetectingTime = DateTime.MinValue;
        private SlingShotController _slingShotController;
        private bool _isLastBootBeenDestroyed = false;
        private void Start()
        {
            SetStartingLives();
            SetSlingShotController();
            _rect = new Rect(x: Screen.width / 2, y: Screen.height / 2, width: Screen.width / 6,
                height: Screen.height / 4);
            _guiStyle = new GUIStyle {fontSize = 40, fontStyle = FontStyle.BoldAndItalic};
        }

        private void SetSlingShotController()
        {
            if (GameObject.FindGameObjectWithTag(TagUtil.SlingShot) == null)
            {
                throw new MissingTagException(TagUtil.SlingShot);
            }

            if (GameObject.FindGameObjectWithTag(TagUtil.SlingShot).GetComponent<SlingShotController>() == null)
            {
                throw new CustomMissingComponentException(TagUtil.SlingShotController);
            }
            _slingShotController = GameObject.FindGameObjectWithTag(TagUtil.SlingShot).GetComponent<SlingShotController>();
        }

        private static void SetStartingLives()
        {
            if (!GlobalConfiguration.IsDevMode())
            {
                if (_previousSceneBuildIndex == SceneManager.GetActiveScene().buildIndex) return;
                _lives = 3;
                _previousSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            }
            else
            {
                _lives = 3;
            }
        }


        private void Update()
        {
            if (_isLost) return;
            if (_isWin) return;
            if (_lives != 0 || _isGoal || !_isLastBootBeenDestroyed) return;
            if (_noGoalDetectingTime == DateTime.MinValue)
                _noGoalDetectingTime = DateTime.Now;
            var currentTime = DateTime.Now;
            var timeDifference = (int) (currentTime - _noGoalDetectingTime).TotalMilliseconds;
            if (timeDifference <= 2000) return;
            _isLost = true;
            if (GlobalConfiguration.IsDevMode())
            {
                StartCoroutine(ReturnObjectsToStartPositions());
            }

        }

        public bool DecreaseLife()
        {
            _lives--;
            if (_lives <= 0 || _isGoal || _isWin) return false;
            var boot = _slingShotController.Boot = Instantiate(BootPrefab);
            CameraController.SetNewBoot(boot);
            return true;

        }

        public void SetLastBootDestroyed()
        {
            _isLastBootBeenDestroyed = true;
        }

        private static void UpdateScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private static IEnumerator ReturnObjectsToStartPositions()
        {
            yield return new WaitForSeconds(2);
            UpdateScene();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null || !other.gameObject.tag.Equals(TagUtil.Ball)) return;
            _isGoal = true;
            GoToNextLevel();
        }

        private void OnGUI()
        {
            if (_isGoal)
            {
                GUI.Label(_rect, "GOAL!", _guiStyle);
            }

            if (_isWin)
            {
                GUI.Label(_rect, "Oh yeah zaebok!", _guiStyle);
            }

            if (_isLost)
            {
                GUI.Label(_rect, "Proeb", _guiStyle);
            }
            else if (!_isGoal)
            {
                GUI.Label(_rect, "Remainig " + _lives + " lifes", _guiStyle);
            }
        }

        private void GoToNextLevel()
        {
            StartCoroutine(NextLevel());
        }

        private IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(2);
            if (LevelUtil.LoadNextLevel()) yield break;
            _isGoal = false;
            _isWin = true;
        }
    }
}