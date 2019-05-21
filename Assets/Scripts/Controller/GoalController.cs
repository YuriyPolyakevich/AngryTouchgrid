using System;
using System.Collections;
using UnityEngine;
using Util;

namespace Controller
{
    public class GoalController : MonoBehaviour
    {

        private bool _isGoal = false;
        private bool _isFinish = false;
        private GUIStyle _guiStyle;

        private void Start()
        {
            _guiStyle = new GUIStyle {fontSize = 40};
        }

        
        private void OnTriggerEnter(Collider other)
        {
            if (other == null || !other.gameObject.tag.Equals("Ball")) return;
            _isGoal = true;
            GoToNextLevel();
        }

        private void OnGUI()
        {
            var rect = new Rect(x: Screen.width / 2, y: Screen.height / 2, width: Screen.width / 6,
                height: Screen.height / 4);
            if (_isGoal)
            {
                GUI.Label(rect, "GOAL!", _guiStyle);
            }

            if (_isFinish)
            {
                GUI.Label(rect, "Oh yeah zaebok!", _guiStyle);
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
                Console.WriteLine(e);
                _isGoal = false;
                _isFinish = true;
            }
            
        }

        public bool IsGoal()
        {
            return _isGoal;
        }
        
        
    }
    
}