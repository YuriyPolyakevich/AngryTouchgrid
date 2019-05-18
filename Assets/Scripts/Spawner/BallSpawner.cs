using System.Collections;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ball;

    private Vector3 _thisTransformPosition;
    private bool _isNewGame;
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
    
    
}