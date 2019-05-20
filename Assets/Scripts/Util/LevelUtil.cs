using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class LevelUtil : MonoBehaviour
    {
        private static int CurrentLevel { get; set; }
        public Scene[] Scenes;

        private void Start()
        {
            CurrentLevel = 1;
        }

        public static void LoadNextLevel()
        {
            SceneManager.LoadScene(GetSceneName());
        }

        private static string GetSceneName()
        {
            return "1";
        }
    }
}