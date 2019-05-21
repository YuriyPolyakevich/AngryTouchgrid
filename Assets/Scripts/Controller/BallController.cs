using UnityEngine;

namespace Controller
{
	public class BallController : MonoBehaviour
	{

		
		private Rigidbody _rigidBody;
		private const float ToleranceValue = 0.00005f;
		private bool _isBallKicked;
		private bool _noGoal = false;
		private GUIStyle _guiStyle;
		private GoalController _goalController;
		// Use this for initialization
		private void Start ()
		{
			_goalController = GameObject.FindGameObjectWithTag("GoalController").GetComponent<GoalController>();
			_guiStyle = new GUIStyle {fontSize = 40};
			_rigidBody = GetComponent<Rigidbody>();
		}
	
		// Update is called once per frame
		void Update () {
			if (_isBallKicked && _rigidBody.velocity.x < ToleranceValue 
			                  && _rigidBody.velocity.y < ToleranceValue
			                  && _rigidBody.velocity.z < ToleranceValue
			                  && !_goalController.IsGoal())
			{
				_noGoal = true;
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other == null || !other.gameObject.transform.tag.Equals("Player")) return;
			_rigidBody.useGravity = true;
			_isBallKicked = true;
		}
		
		private void OnGUI()
		{
			if (!_noGoal) return;
			var rect = new Rect(x: Screen.width / 2, y: Screen.height / 2, width: Screen.width / 6,
				height: Screen.height / 4);
			GUI.Label(rect, "Хуйня, ты проебал", _guiStyle);
		}
	}
}
