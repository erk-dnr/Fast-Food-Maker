using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
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
    [SerializeField] Transform lobbyListContainer;
    [SerializeField] Transform lobbyListEntryTemplate;

    void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
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
        
        lobbyListEntryTemplate.gameObject.SetActive(false);
    }

    void Start()
    {
        playerNameInput.text = KitchenGameMultiplayer.Instance.PlayerName;
        playerNameInput.onValueChanged.AddListener((string newName) =>
        {
            KitchenGameMultiplayer.Instance.PlayerName = newName;
        });
        
        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }

    void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyListContainer)
        {
            if (child == lobbyListEntryTemplate) 
                continue;
            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyListEntryTemplate, lobbyListContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
        
    }
}
