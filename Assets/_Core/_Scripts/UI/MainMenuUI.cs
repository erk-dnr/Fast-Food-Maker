using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] Button playButton;
    [SerializeField] Button quitButton;

    void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.LobbyScene);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        // reset time scale after pausing the game and going back to MainMenu
        Time.timeScale = 1f;
    }
}
