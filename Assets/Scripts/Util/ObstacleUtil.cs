using Model.Enumeration;
using UnityEngine;

namespace Util
{
    public static class ObstacleUtil
    {
        public static GameObject rectangle;
        public static GameObject square;
        public static GameObject sphere;
        public static GameObject GetObjectByType(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.RECTANGLE: return rectangle;
                case ObstacleType.SPHERE: return sphere;
                case ObstacleType.SQUARE: return square;
                default: return null;
            }
        }
    }
}