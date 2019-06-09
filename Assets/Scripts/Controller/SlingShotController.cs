using Configuration.Exception;
using Controller;
using UnityEngine;
using Util;

public class SlingShotController : MonoBehaviour
{
    public GameObject Boot {private get; set; }
    private Rigidbody _rigidBody;
    private BootController _bootController;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag(TagUtil.Player) == null)
        {
            throw new MissingTagException(TagUtil.Player);
        }

        if (GameObject.FindGameObjectWithTag(TagUtil.Player).GetComponent<BootController>() == null)
        {
            throw new CustomMissingComponentException(TagUtil.BootController);
        }
        Boot = GameObject.FindGameObjectWithTag(TagUtil.Player);
        _bootController = Boot.GetComponent<BootController>();
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

        if (_bootController.IsBootMoving && _rigidBody.isKinematic)
        {
            _rigidBody.isKinematic = false;
        }
    }

}