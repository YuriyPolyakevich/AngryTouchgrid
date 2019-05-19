using System.Collections.Generic;
using Model;
using UnityEngine;
using Random = System.Random;

namespace Util
{
    //todo: transfer to database
    public static class EndlessLevelUtil
    {
        private static int _levelCount = 1;
        private static readonly List<EndlessLevel> EndlessLevels = new List<EndlessLevel>();
        private static GameObject ball;
            
        public static void GenerateEndlessLevels(GameObject initialBall)
        {
            ball = initialBall;
            GenerateFirstLevel();
            GenerateSecondLevel();
            GenerateThirdLevel();
            GenerateFourthLevel();
            GenerateFifthLevel();
            GenerateSixthLevel();
            GenerateSeventhLevel();
            GenerateEightLevel();
            GenerateNinthLevel();
            GenerateTenthLevel();
        }

        private static void GenerateTenthLevel()
        {
        }

        private static void GenerateNinthLevel()
        {
        }

        private static void GenerateEightLevel()
        {
        }

        private static void GenerateSeventhLevel()
        {
        }

        private static void GenerateSixthLevel()
        {
        }

        private static void GenerateFifthLevel()
        {
        }

        private static void GenerateFourthLevel()
        {
        }

        private static void GenerateThirdLevel()
        {
        }

        private static void GenerateSecondLevel()
        {
            var position = ball.transform.position;
            var obstaclePosition = new Vector3(position.x - 10, 
                position.y, position.z);
            var obstacle = CreateObstacle(
                obstaclePosition, 4, Vector3.up, 0, 10);
            var obstacles = new List<Obstacle> {obstacle};
            var endlessLevel = new EndlessLevel
            {
                BallSpeed = 0,
                BallDirection = Vector3.zero,
                BallMovementDistance = Vector3.zero,
                Obstacles = obstacles
            };
            EndlessLevels.Add(endlessLevel);
        }

        private static void GenerateFirstLevel()
        {
            var endlessLevel = new EndlessLevel
            {
                BallSpeed = 0,
                BallDirection = Vector3.zero,
                BallMovementDistance = Vector3.zero
            };
            EndlessLevels.Add(endlessLevel);
        }

        private static Obstacle CreateObstacle(Vector3 position, float movementDistance, Vector3 direction,
            float rotationSpeed, float speed)
        {
            var obstacle = new Obstacle
            {
                Position = position,
                MovementDistance = movementDistance,
                Direction = direction,
                RotationSpeed = rotationSpeed,
                Speed = speed
            };
            return obstacle;
        }

        public static int GetCurrentLevelCount()
        {
            return _levelCount;
        }

        public static EndlessLevel GetCurrentLevel()
        {
            return EndlessLevels[_levelCount];
        }

        public static EndlessLevel GetNextLevel()
        {
            if (_levelCount < EndlessLevels.Count) return EndlessLevels[_levelCount];
            var random = new Random();
            return EndlessLevels[random.Next(0, EndlessLevels.Count - 1)];
        }
    }
}