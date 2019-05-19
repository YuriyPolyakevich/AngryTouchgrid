using System.Collections;
using System.Collections.Generic;
using Model.GameObjectModel;
using UnityEngine;
using Util;

public class BallSpawner : MonoBehaviour
{
    public GameObject ball;
    private List<GameObjectObstacle> _currentStaff = new List<GameObjectObstacle>();
    private Vector3 _thisTransformPosition;
    private bool _isNewGame;

    private bool _isGameStarted;
    // Update is called once per frame

    private void Start()
    {
        _thisTransformPosition = transform.position;
        transform.LookAt(Vector3.down);
        _isNewGame = true;
    }

    private void Update()
    {
//        Instantiate(ball, transform.position, Quaternion.identity);
        if (!_isNewGame) return;
        _isNewGame = false;
        StartCoroutine(NewGame());
    }
    
    private IEnumerator NewGame()
    {
        ball.transform.position = _thisTransformPosition;
        yield return new WaitForSeconds(2);
        Instantiate(ball);
    }

    public void GoToNextLevel()
    {
        foreach (var i in _currentStaff)
        {
            Destroy(i.GameObject);
        }
        _currentStaff.Clear();
        var endlessLevel = EndlessLevelUtil.GetNextLevel();
        ball.transform.position = endlessLevel.BallPosition;
        foreach (var obstacle in endlessLevel.Obstacles)
        {
            var obj = ObstacleUtil.GetObjectByType(obstacle.Type);
            obj.transform.position = obstacle.Position;
            obj.transform.LookAt(obstacle.Direction);
            var gameObjectObstacle = new GameObjectObstacle(Instantiate(obj), obstacle);
            _currentStaff.Add(gameObjectObstacle);
        }
    }
    
    
}