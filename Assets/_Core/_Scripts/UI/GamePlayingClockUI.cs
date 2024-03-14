using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{

    [SerializeField] Image timerImage;

    void Start()
    {
        timerImage.fillAmount = 0f;
    }

    void Update()
    {
        timerImage.fillAmount = GameManager.Instance.GetGameplayTimerNormalized();
    }
}
