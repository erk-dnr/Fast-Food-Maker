using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{

    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] OptionsUI optionsUI;


    void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });
        
        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            optionsUI.Show(Show);
        });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameResumed += GameManager_OnLocalGameResumed;
        
        Hide();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnLocalGamePaused -= GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameResumed -= GameManager_OnLocalGameResumed;
    }

    void GameManager_OnLocalGamePaused(object sender, EventArgs e)
    {
        Show();
    }
    
    void GameManager_OnLocalGameResumed(object sender, EventArgs e)
    {
        Hide();
    }

    void Show()
    {
        gameObject.SetActive(true);
        
        resumeButton.Select();
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
