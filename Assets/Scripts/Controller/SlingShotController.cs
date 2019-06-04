using Controller;
using UnityEngine;

public class SlingShotController : MonoBehaviour
{
    public GameObject Boot {private get; set; }
    private Rigidbody _rigidBody;
    private BootController _bootController;

    private void Start()
    {
        Boot = GameObject.FindGameObjectWithTag("Player");
        _bootController = Boot.GetComponent<BootController>();
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