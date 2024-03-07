using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

public class ProgressBarUI : MonoBehaviour
{

    [SerializeField] Image barImage;
    [SerializeField] GameObject hasProgressGameObject;
    
    IHasProgress _hasProgress;

    void Start()
    {
        _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (_hasProgress == null)
        {
            Debug.LogError($"GameObject {hasProgressGameObject} does not have a component that implements IHasProgress");
        }
        
        _hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    void OnDestroy()
    {
        _hasProgress.OnProgressChanged -= HasProgress_OnProgressChanged;
    }

    async void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f)
        {
            Hide();
        }
        else if (e.progressNormalized == 1f)
        {
            // hide progress bar after short delay when cutting process is finished
            await Task.Delay(500);
            Hide();
        }
        else
        {
            Show();
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
