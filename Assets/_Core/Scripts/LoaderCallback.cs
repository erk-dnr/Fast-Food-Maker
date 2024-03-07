using System;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{

    bool _isFirstUpdate = true;

    void Update()
    {
        if (_isFirstUpdate)
        {
            _isFirstUpdate = false;

            Loader.LoaderCallback();
        }
    }
}
