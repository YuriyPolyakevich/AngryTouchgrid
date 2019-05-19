using UnityEngine;

namespace Controller
{
	public class BallController : MonoBehaviour
	{

		private Rigidbody _rigidBody;
		private const float ToleranceValue = 0.005f;
		private bool _isBallKicked;

		// Use this for initialization
		private void Start ()
		{
			_rigidBody = GetComponent<Rigidbody>();
		}
	
		// Update is called once per frame
		void Update () {
			if (_isBallKicked && _rigidBody.velocity.x < ToleranceValue 
			                  && _rigidBody.velocity.y < ToleranceValue
			                  && _rigidBody.velocity.z < ToleranceValue)
			{
				Debug.unityLogger.Log("-1 life");
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other == null || !other.gameObject.transform.tag.Equals("Player")) return;
			_rigidBody.useGravity = true;
			_isBallKicked = true;
		}
	}
}
