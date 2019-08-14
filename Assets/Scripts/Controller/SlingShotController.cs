using Configuration.Exception;
using Controller;
using UnityEngine;
using Util;

public class SlingShotController : MonoBehaviour
{
    
    private Rigidbody _rigidBody;
    public BootPerspectiveController BootPerspectiveController { private get; set; }
    public GameObject Boot { private get; set; }

    private void Start()
    {
        
        if (GetComponent<Rigidbody>() == null)
        {
            throw new CustomMissingComponentException(TagUtil.RigidBody);
        }
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_rigidBody.isKinematic)
        {
            transform.position = Boot.transform.position;
        }

        if (BootPerspectiveController.IsBootMoving && _rigidBody.isKinematic)
        {
            SetKinematic(false);
        }
    }


    private void SetKinematic(bool isKinematic)
    {
        _rigidBody.isKinematic = isKinematic;
    }

}