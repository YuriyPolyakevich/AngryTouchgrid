using System;
using UnityEngine;

namespace Controller
{
	public class BallController : MonoBehaviour
	{

		
		private Rigidbody _rigidBody;
		private GoalController _goalController;

		private void Start ()
		{
			_goalController = GameObject.FindGameObjectWithTag("GoalController").GetComponent<GoalController>();
			_rigidBody = GetComponent<Rigidbody>();
			_rigidBody.constraints = RigidbodyConstraints.FreezePositionZ;
		}
	
		private void OnCollisionEnter(Collision other)
		{
			if (other == null || !other.gameObject.transform.tag.Equals("Player")) return;
			_rigidBody.useGravity = true;
			_goalController.IsBallKicked = true;
			_goalController.BallKickedTime = DateTime.Now;
		}
		
	}
}
