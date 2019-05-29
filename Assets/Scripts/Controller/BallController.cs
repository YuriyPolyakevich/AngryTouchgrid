using UnityEngine;

namespace Controller
{
	public class BallController : MonoBehaviour
	{

		
		private Rigidbody _rigidBody;
		private const float ToleranceValue = 0.00005f;
		private bool _noGoal = false;
		private GUIStyle _guiStyle;
		private GoalController _goalController;
		public Vector3 InitialPosition { get; private set; }
		// Use this for initialization
		private void Start ()
		{
			InitialPosition = transform.position;
			_goalController = GameObject.FindGameObjectWithTag("GoalController").GetComponent<GoalController>();
			_guiStyle = new GUIStyle {fontSize = 40};
			_rigidBody = GetComponent<Rigidbody>();
			_rigidBody.constraints = RigidbodyConstraints.FreezePositionZ;
		}
	
		private void OnCollisionEnter(Collision other)
		{
			if (other == null || !other.gameObject.transform.tag.Equals("Player")) return;
			_rigidBody.useGravity = true;
			_goalController.IsBallKicked = true;
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
