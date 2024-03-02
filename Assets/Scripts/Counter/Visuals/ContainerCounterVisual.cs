using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ContainerCounterVisual : MonoBehaviour
{

    [SerializeField] ContainerCounter containerCounter;

    const string OPEN_CLOSE = "OpenClose";
    
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
    }

    void OnDestroy()
    {
        containerCounter.OnPlayerGrabbedObject -= ContainerCounter_OnPlayerGrabbedObject;
    }

    void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e)
    {
        _animator.SetTrigger(OPEN_CLOSE);
    }
}
