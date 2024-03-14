using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{

    [SerializeField] Button mainMenuButton;
    [SerializeField] Button readyButton;
    [SerializeField] TextMeshProUGUI lobbyNameText;
    [SerializeField] TextMeshProUGUI lobbyCodeText;

    void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    void Start()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();

        lobbyNameText.text = $"Lobby Name: {lobby.Name}";
        lobbyCodeText.text = $"Lobby Code: {lobby.LobbyCode}";
    }
}
