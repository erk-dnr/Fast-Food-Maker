using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StoveBurnFlashingBarUI : MonoBehaviour
{

    [SerializeField] StoveCounter stoveCounter;

    const string IS_FLASHING = "IsFlashing";
    static readonly int IsFlashing = Animator.StringToHash(IS_FLASHING);

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        _animator.SetBool(IsFlashing, false);
    }

    void OnDestroy()
    {
        stoveCounter.OnProgressChanged -= StoveCounter_OnProgressChanged;
    }

    void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        bool flash = stoveCounter.IsFried() && e.progressNormalized >= stoveCounter.BurnWarningThreshold;
        _animator.SetBool(IsFlashing, flash);
    }
}
