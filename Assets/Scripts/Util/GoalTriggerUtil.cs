using UnityEngine;

namespace Util
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