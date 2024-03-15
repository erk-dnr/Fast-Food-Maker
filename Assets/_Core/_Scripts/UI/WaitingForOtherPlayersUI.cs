using System;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        
        Hide();
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownActive)
        {
            Hide();
        }
    }

    void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady)
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
