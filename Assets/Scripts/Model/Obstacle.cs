using Model.Enumeration;
using UnityEngine;

namespace Model
{
    public class Obstacle
    {
        public Vector3 Position { get; set; }
        public float MovementDistance { get; set; }
        public Vector3 Direction { get; set; }
        public float RotationSpeed { get; set; }
        public float Speed { get; set; }

        public ObstacleType Type { get; set ; }
    }
}