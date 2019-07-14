using System;
using Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class LevelUtil : MonoBehaviour
    {
        private static int CurrentLevel { get; set; }
        private static int CurrentLocation { get; set; }

        private void Start()
        {
            CurrentLevel = 1;
            CurrentLocation = 1;
        }

        public static bool LoadNextLevel(int livesRemained)
        {
            var currentScene = SceneManager.GetActiveScene().name;
            GetCurrentLocationAndCurrentLevel(currentScene);
            CurrentLevel++;
            var sceneName = GetSceneName();
            if (GlobalConfiguration.IsDevMode())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return true;
            }
            if (!Application.CanStreamedLevelBeLoaded(sceneName) && !GlobalConfiguration.IsDevMode()) return false;
            SceneManager.LoadScene(sceneName);
            return true;
        }

        private static void GetCurrentLocationAndCurrentLevel(string currentScene)
        {
            var splittedSceneName = currentScene.Split('.');
            if (splittedSceneName.Length < 4) throw new Exception("Wrong Scene Name: '" + currentScene + "'");
            CurrentLevel = int.Parse(splittedSceneName[splittedSceneName.Length - 2]);
            CurrentLocation = int.Parse(splittedSceneName[splittedSceneName.Length - 3]);
        }

        private static string GetSceneName()
        {
            var scenePostfix = GlobalConfiguration.IsDevMode() ? "dev" : "prod";
            return "level." + CurrentLocation + "." + CurrentLevel + "." + scenePostfix;
        }
    }
}