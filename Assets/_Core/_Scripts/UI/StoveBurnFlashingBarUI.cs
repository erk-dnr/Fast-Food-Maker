using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StoveBurnFlashingBarUI : MonoBehaviour
{

    [SerializeField] StoveCounter stoveCounter;

    const string IS_FLASHING = "IsFlashing";

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        _animator.SetBool(IS_FLASHING, false);
    }

    void OnDestroy()
    {
        stoveCounter.OnProgressChanged -= StoveCounter_OnProgressChanged;
    }

    void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        bool flash = stoveCounter.IsFried() && e.progressNormalized >= stoveCounter.BurnWarningThreshold;
        _animator.SetBool(IS_FLASHING, flash);
    }
}
