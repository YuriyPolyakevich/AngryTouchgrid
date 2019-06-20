using Configuration.Exception;
using Controller;
using UnityEngine;
using Util;

public class SlingShotController : MonoBehaviour
{
    
    private Rigidbody _rigidBody;
    public BootController BootController { get; set; }
    public GameObject Boot { get; set; }

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

        if (BootController.IsBootMoving && _rigidBody.isKinematic)
        {
            _rigidBody.isKinematic = false;
        }
    }


}