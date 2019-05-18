using UnityEngine;

namespace a
{
    public class Obstacle
    {
        public Vector3 Position { get; set; }
        public float MovementDistance { get; set; }
        public Vector3 Direction { get; set; }
        public float RotationSpeed { get; set; }
        public float Speed { get; set; }
    }
}