using UnityEngine;

namespace a
{
    public class GoalTriggerUtil: MonoBehaviour
    {
        private void OnTriggerEnter(Collider objectCollider)
        {
            if (objectCollider != null && objectCollider.gameObject.tag.Equals("Ball"))
            {
                Debug.unityLogger.Log("GOAAAALLALALALALALALLAL");
            }
        }
    }
}