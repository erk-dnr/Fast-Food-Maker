using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    [SerializeField] Mode mode;
    
    Camera _camera;

    void Awake()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(_camera.transform);
                break;
            case Mode.LookAtInverted:
                var position = transform.position;
                Vector3 dirFromCamera = position - _camera.transform.position;
                transform.LookAt(position + dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = _camera.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -_camera.transform.forward;
                break;
        }
    }
}
