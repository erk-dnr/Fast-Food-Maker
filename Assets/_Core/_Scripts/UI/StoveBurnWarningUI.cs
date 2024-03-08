using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] StoveCounter stoveCounter;

    void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        Hide();
    }

    void OnDestroy()
    {
        stoveCounter.OnProgressChanged -= StoveCounter_OnProgressChanged;
    }

    void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        bool show = stoveCounter.IsFried() && e.progressNormalized >= stoveCounter.BurnWarningThreshold;
        gameObject.SetActive(show);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
