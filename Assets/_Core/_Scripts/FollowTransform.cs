using System;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{

    Transform _thisTransform;
    Transform _targetTransform;

    void LateUpdate()
    {
        if (_targetTransform == null)
        {
            return;
        }

        _thisTransform = transform;
        _thisTransform.position = _targetTransform.position;
        _thisTransform.rotation = _targetTransform.rotation;
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        _targetTransform = targetTransform;
    }
}
