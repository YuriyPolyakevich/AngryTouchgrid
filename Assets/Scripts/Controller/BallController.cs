using Configuration.Exception;
using UnityEngine;
using Util;

namespace Controller
{
	public class BallController : MonoBehaviour
	{
		private Rigidbody _rigidBody;

		private void Start ()
		{
			if (GetComponent<Rigidbody>() == null)
			{
				throw new CustomMissingComponentException(TagUtil.RigidBody);
			}
			_rigidBody = GetComponent<Rigidbody>();
			_rigidBody.constraints = RigidbodyConstraints.FreezePositionZ;
		}


		private void OnCollisionEnter(Collision other)
		{
			if (other == null || !other.gameObject.transform.tag.Equals("Player")) return;
			_rigidBody.useGravity = true;
		}
		
	}
}
