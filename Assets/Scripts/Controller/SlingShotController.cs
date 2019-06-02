using Controller;
using UnityEngine;

public class SlingShotController : MonoBehaviour
{
    private GameObject _boot;
    private Rigidbody _rigidbody;
    private BootController _bootController;

    private void Start()
    {
        _boot = GameObject.FindGameObjectWithTag("Player");
        _bootController = _boot.GetComponent<BootController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_rigidbody.isKinematic)
        {
            transform.position = _boot.transform.position;
        }

        if (_bootController.IsBootMoving && _rigidbody.isKinematic)
        {
            _rigidbody.isKinematic = false;
        }
    }
}