using UnityEngine;

namespace Controller
{
    public class GoalController : MonoBehaviour
    {

        private void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.unityLogger.Log(other);
            if (other == null || !other.gameObject.tag.Equals("Ball")) return;
            Debug.unityLogger.Log("GOALALAOAAL");
        }
    }
}