using System;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{

    void Awake()
    {
        GameManager.Instance.OnMultiplayerGamePaused += GameManager_OnMultiplayerGamePaused;
        GameManager.Instance.OnMultiplayerGameResumed += GameManager_OnMultiplayerGameResumed;
        
        Hide();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnMultiplayerGamePaused -= GameManager_OnMultiplayerGamePaused;
        GameManager.Instance.OnMultiplayerGameResumed -= GameManager_OnMultiplayerGameResumed;
    }

    void GameManager_OnMultiplayerGamePaused(object sender, EventArgs e)
    {
        // only show this screen for unpaused clients
        if (!GameManager.Instance.IsLocalGamePaused)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    void GameManager_OnMultiplayerGameResumed(object sender, EventArgs e)
    {
        Hide();
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
