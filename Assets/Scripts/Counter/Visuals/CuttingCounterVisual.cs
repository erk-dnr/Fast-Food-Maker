using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CuttingCounterVisual : MonoBehaviour
{

    [SerializeField] CuttingCounter cuttingCounter;

    const string CUT = "Cut";
    
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        cuttingCounter.OnCut += CuttingCounter_OnCut;
    }

    void OnDestroy()
    {
        cuttingCounter.OnCut -= CuttingCounter_OnCut;
    }

    void CuttingCounter_OnCut(object sender, EventArgs e)
    {
        _animator.SetTrigger(CUT);
    }
}
