using System.Collections.Generic;
using UnityEngine;

namespace a
{
    public class EndlessLevel
    {
        public List<Obstacle> Obstacles { get; set; }
        public Vector3 BallPosition { get; set; }
        public Vector3 BallDirection { get; set; }
        public float BallSpeed { get; set; }
        public Vector3 BallMovementDistance { get; set; }
    }
}