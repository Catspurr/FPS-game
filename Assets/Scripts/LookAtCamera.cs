using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _cam;

    private void Update()
    {
        if (_cam == null) _cam = FindObjectOfType<Camera>();
        if (_cam == null) return;
        
        transform.LookAt(_cam.transform);
    }
}