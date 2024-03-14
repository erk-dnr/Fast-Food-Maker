using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLobby : MonoBehaviour
{
    
    public static GameLobby Instance { get; private set; }

    [SerializeField] float heartbeatInterval = 15f;

    Lobby _joinedLobby;
    float _heartbeatTimer;
    
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        
        DontDestroyOnLoad(gameObject);
        
        InitializeUnityAuthentication();
    }
    
    void Update()
    {
        HandleHeartbeat();
    }

    bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 1000).ToString());
            
            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0f)
            {
                _heartbeatTimer = heartbeatInterval;
                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyName,
                KitchenGameMultiplayer.Instance.MaxPlayerAmount,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });
            
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void JoinByCode(string lobbyCode)
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Lobby GetLobby()
    {
        return _joinedLobby;
    }
        
}
