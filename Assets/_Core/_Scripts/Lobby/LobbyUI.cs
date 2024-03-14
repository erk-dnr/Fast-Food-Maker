using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] Button mainMenuButton;
    [SerializeField] Button createLobbyButton;
    [SerializeField] Button quickJoinButton;
    [SerializeField] Button joinCodeButton;
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] LobbyCreateUI lobbyCreateUI;

    void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinByCode(joinCodeInput.text);
        });
    }

    void Start()
    {
        playerNameInput.text = KitchenGameMultiplayer.Instance.PlayerName;
        playerNameInput.onValueChanged.AddListener((string newName) =>
        {
            KitchenGameMultiplayer.Instance.PlayerName = newName;
        });
    }
}
