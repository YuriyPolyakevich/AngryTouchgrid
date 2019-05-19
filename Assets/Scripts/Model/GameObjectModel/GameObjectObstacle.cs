using UnityEngine;

namespace Model.GameObjectModel
{
    public class GameObjectObstacle
    {
        public GameObject GameObject { get; set; }
        public Obstacle Obstacle { get; set; }

        public GameObjectObstacle(GameObject gameObject, Obstacle obstacle)
        {
            Obstacle = obstacle;
            GameObject = gameObject;
        }
    }
}