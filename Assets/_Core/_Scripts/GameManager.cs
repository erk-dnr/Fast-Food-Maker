using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged; 
    public event EventHandler OnGamePaused; 
    public event EventHandler OnGameResumed; 
    
    enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
    
    [SerializeField] float countdownTimerMax = 3f;
    [SerializeField] float gameplayTimerMax = 300f;
    
    float _countdownTimer;
    float _gameplayTimer;
    bool _isGamePaused = false;

    State _state;

    public bool IsGamePlaying => _state == State.GamePlaying;
    public bool IsCountdownActive => _state == State.CountdownToStart;
    public bool IsGameOver => _state == State.GameOver;

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

    }

    void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPause;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        
        // _state = State.WaitingToStart;
        // DEBUG TRIGGER GAME START AUTOMATICALLY
        _state = State.CountdownToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        
        _countdownTimer = countdownTimerMax;
        _gameplayTimer = gameplayTimerMax;
    }

    void OnDestroy()
    {
        GameInput.Instance.OnPauseAction -= GameInput_OnPause;
        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
    }

    void Update()
    {
        switch (_state)
        {
            case State.WaitingToStart:
                // wait for player pressing Interact button
                break;
            case State.CountdownToStart:
                _countdownTimer -= Time.deltaTime;
                if (_countdownTimer <= 0f)
                {
                    _state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GamePlaying:
                _gameplayTimer -= Time.deltaTime;
                if (_gameplayTimer <= 0f)
                {
                    _state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GameOver:
                break;
        }
    }
    
    void GameInput_OnPause(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (_state == State.WaitingToStart)
        {
            _state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void TogglePauseGame()
    {
        _isGamePaused = !_isGamePaused;
        if (_isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameResumed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public float GetCountdownTimer()
    {
        return _countdownTimer;
    }

    public float GetGameplayTimerNormalized()
    {
        return 1f - (_gameplayTimer / gameplayTimerMax);
    }
}
