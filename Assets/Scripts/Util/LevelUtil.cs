using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class LevelUtil : MonoBehaviour
    {
        private static int CurrentLevel { get; set; }
        private static int CurrentLocation { get; set; }
        public Scene[] Scenes;

        private void Start()
        {
            CurrentLevel = 1;
            CurrentLocation = 1;
        }

        public static void LoadNextLevel()
        {
            CurrentLevel++;
            SceneManager.LoadScene(GetSceneName());
        }

        private static string GetSceneName()
        {
            return "level" + CurrentLocation + "." + CurrentLevel;
        }
    }
}