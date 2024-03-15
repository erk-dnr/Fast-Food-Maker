using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged; 
    public event EventHandler OnLocalGamePaused; 
    public event EventHandler OnLocalGameResumed;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameResumed;
    public event EventHandler OnLocalPlayerReadyChanged;
    
    enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    [SerializeField] Transform playerPrefab;
    [SerializeField] float countdownTimerMax = 3f;
    [SerializeField] float gameplayTimerMax = 120f;
    
    NetworkVariable<State> _state = new NetworkVariable<State>(State.WaitingToStart);
    NetworkVariable<float> _countdownTimer = new NetworkVariable<float>();
    NetworkVariable<float> _gameplayTimer = new NetworkVariable<float>();
    NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);
    
    bool _isLocalGamePaused;
    bool _isLocalPlayerReady;
    bool _autoCheckGamePauseState;
    Dictionary<ulong, bool> _playerReadyDictionary;
    Dictionary<ulong, bool> _playerPausedDictionary;

    public bool IsWaitingToStart => _state.Value == State.WaitingToStart;
    public bool IsCountdownActive => _state.Value == State.CountdownToStart;
    public bool IsGamePlaying => _state.Value == State.GamePlaying;
    public bool IsGameOver => _state.Value == State.GameOver;
    public bool IsLocalPlayerReady => _isLocalPlayerReady;
    
    public bool IsLocalGamePaused => _isLocalGamePaused;

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
        _playerReadyDictionary = new Dictionary<ulong, bool>();
        _playerPausedDictionary = new Dictionary<ulong, bool>();

    }

    void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPause;
        GameInput.Instance.OnInteractAction += GameInput_OnInteract;
    }

    public override void OnNetworkSpawn()
    {
        _state.OnValueChanged += State_OnValueChanged;
        _isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
        
        _countdownTimer.Value = countdownTimerMax;
        _gameplayTimer.Value = gameplayTimerMax;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkSceneManager_OnLoadEventCompleted;
        }
    }

    public override void OnNetworkDespawn()
    {
        _state.OnValueChanged -= State_OnValueChanged;
        _isGamePaused.OnValueChanged -= IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= NetworkSceneManager_OnLoadEventCompleted;
        }
    }

    public override void OnDestroy()
    {
        GameInput.Instance.OnPauseAction -= GameInput_OnPause;
        GameInput.Instance.OnInteractAction -= GameInput_OnInteract;
    }

    void Update()
    {
        if (!IsServer)
            return;
        
        switch (_state.Value)
        {
            case State.WaitingToStart:
                // wait for player pressing Interact button
                break;
            case State.CountdownToStart:
                _countdownTimer.Value -= Time.deltaTime;
                if (_countdownTimer.Value <= 0f)
                {
                    _state.Value = State.GamePlaying;
                    _gameplayTimer.Value = gameplayTimerMax;
                }

                break;
            case State.GamePlaying:
                _gameplayTimer.Value -= Time.deltaTime;
                if (_gameplayTimer.Value <= 0f)
                {
                    _state.Value = State.GameOver;
                }

                break;
            case State.GameOver:
                break;
        }
    }

    void LateUpdate()
    {
        if (_autoCheckGamePauseState)
        {
            _autoCheckGamePauseState = false;
            CheckGamePausedState();
        }
    }

    void GameInput_OnPause(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    void GameInput_OnInteract(object sender, EventArgs e)
    {
        if (_state.Value == State.WaitingToStart)
        {
            _isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }
    
    void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    
    void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (_isGamePaused.Value)
        {
            Time.timeScale = 0f;
            
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            
            OnMultiplayerGameResumed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    void NetworkManager_OnClientDisconnect(ulong clientId)
    {
        _autoCheckGamePauseState = true;
    }
    
    void NetworkSceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log(serverRpcParams.Receive.SenderClientId);
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        _playerReadyDictionary[senderClientId] = true;

        bool allClientsReady = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                // this player is not ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            _state.Value = State.CountdownToStart;
        }
        
        Debug.Log("All Clients ready: " + allClientsReady);
    }

    public void TogglePauseGame()
    {
        _isLocalGamePaused = !_isLocalGamePaused;
        if (_isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            ResumeGameServerRpc();
            OnLocalGameResumed?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        
        CheckGamePausedState();
    }
    
    [ServerRpc(RequireOwnership = false)]
    void ResumeGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        
        CheckGamePausedState();
    }

    void CheckGamePausedState()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (_playerPausedDictionary.ContainsKey(clientId) && _playerPausedDictionary[clientId])
            {
                // this player is paused
                _isGamePaused.Value = true;
                return;
            }
            
            // all players are unpaused
            _isGamePaused.Value = false;
        }
    }
    
    public float GetCountdownTimer()
    {
        return _countdownTimer.Value;
    }

    public float GetGameplayTimerNormalized()
    {
        return 1f - (_gameplayTimer.Value / gameplayTimerMax);
    }
}
