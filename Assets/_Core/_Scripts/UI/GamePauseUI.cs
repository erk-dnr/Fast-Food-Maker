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
            optionsUI.Show();
        });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameResumed += GameManager_OnGameResumed;
        
        Hide();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGamePaused -= GameManager_OnGamePaused;
        GameManager.Instance.OnGameResumed -= GameManager_OnGameResumed;
    }

    void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        Show();
    }
    
    void GameManager_OnGameResumed(object sender, EventArgs e)
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
