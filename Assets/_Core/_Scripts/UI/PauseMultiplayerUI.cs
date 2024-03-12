using System;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{

    void Start()
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
        // TODO: only show this screen for unpaused clients
        Show();
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
