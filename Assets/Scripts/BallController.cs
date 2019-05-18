using UnityEngine;

public class BallController : MonoBehaviour
{

	private Rigidbody _rigidbody;
	// Use this for initialization
	private void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other != null && other.gameObject.transform.tag.Equals("Player"))
		{
			_rigidbody.useGravity = true;
		}
	}
}
