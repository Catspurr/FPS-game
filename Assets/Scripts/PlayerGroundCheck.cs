using System;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) return;
        _playerController.SetGroundedState(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6) return;
        _playerController.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6) return;
        _playerController.SetGroundedState(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 6) return;
        _playerController.SetGroundedState(true);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.layer == 6) return;
        _playerController.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == 6) return;
        _playerController.SetGroundedState(false);
    }
}