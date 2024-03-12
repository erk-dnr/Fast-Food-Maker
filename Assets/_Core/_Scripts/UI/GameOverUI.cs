using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI recipesDeliveredAmountText;
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] Button playAgainButton; // TODO
    [SerializeField] Button resetHighscoreButton;

    const string PLAYER_PREFS_HIGHSCORE = "Highscore";

    void Awake()
    {
        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        
        resetHighscoreButton.onClick.AddListener(() =>
        {
            ResetHighscore();
        });
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver)
        {
            int recipesCompleted = DeliveryManager.Instance.SuccessfulRecipesAmount;
            recipesDeliveredAmountText.text = recipesCompleted.ToString();

            if (recipesCompleted > PlayerPrefs.GetInt(PLAYER_PREFS_HIGHSCORE, 0))
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_HIGHSCORE, recipesCompleted);
            }
            SetHighscoreText();
            
            Show();
        }
        else
        {
            Hide();
        }
    }

    void ResetHighscore()
    {
        PlayerPrefs.SetInt(PLAYER_PREFS_HIGHSCORE, 0);
        SetHighscoreText();
    }

    void SetHighscoreText()
    {
        highscoreText.text = $"Highscore: {PlayerPrefs.GetInt(PLAYER_PREFS_HIGHSCORE, 0)}";
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
