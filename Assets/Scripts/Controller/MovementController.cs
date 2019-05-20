using UnityEngine;

public class MovementController : MonoBehaviour {
	
	public float MovingSpeed;
	public Vector3 lowestPosition = Vector3.zero;
	public Vector3 highestPosition = Vector3.zero;
	private Vector3 _targetPosition;

	private const float ToleranceValue = 0.00001f;
	// Use this for initialization
	private void Start () {
		if (lowestPosition == Vector3.zero)
		{
			lowestPosition = transform.position;
		}

		if (highestPosition == Vector3.zero)
		{
			highestPosition = transform.position;
		}

		_targetPosition = lowestPosition;
		transform.LookAt(lowestPosition);
	}
	
	// Update is called once per frame
	private void Update () {
		if (transform.position == lowestPosition)
		{
			_targetPosition = highestPosition;
		}

		if (transform.position == highestPosition)
		{
			_targetPosition = lowestPosition;
		}

		if ((lowestPosition != Vector3.zero || highestPosition != Vector3.zero) && MovingSpeed > ToleranceValue)
		{
			transform.position = 
				Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * MovingSpeed);
		}
	}
}
