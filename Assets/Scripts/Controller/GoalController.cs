using System.Collections;
using UnityEngine;
using Util;

namespace Controller
{
    public class GoalController : MonoBehaviour
    {

        private static bool _isGoal = false;
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
            if (!_isGoal) return;
            var rect = new Rect(x: Screen.width / 2, y: Screen.height / 2, width: Screen.width / 6,
                height: Screen.height / 4);
            GUI.Label(rect, "GOAL!", _guiStyle);
        }

        private void GoToNextLevel()
        {
            StartCoroutine(NextLevel());
        }

        private static IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(2);
            LevelUtil.LoadNextLevel();
        }

        public static bool IsGoal()
        {
            return _isGoal;
        }
    }
    
}