using System;
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

        public static void LoadNextLevel()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            GetCurrentLocationAndCurrentLevel(currentScene);
            CurrentLevel++;
            var sceneName = GetSceneName();
            if(!Application.CanStreamedLevelBeLoaded(sceneName)) throw new Exception("End");
            SceneManager.LoadScene(sceneName);
        }

        private static void GetCurrentLocationAndCurrentLevel(string currentScene)
        {
            var splittedSceneName = currentScene.Split('.');
            if (splittedSceneName.Length < 3) throw new Exception("Wrong Scene Name: '" + currentScene + "'");
            CurrentLevel = int.Parse(splittedSceneName[splittedSceneName.Length - 1]);
            CurrentLocation = int.Parse(splittedSceneName[splittedSceneName.Length - 2]);
        }

        private static string GetSceneName()
        {
            return "level." + CurrentLocation + "." + CurrentLevel;
        }
    }
}