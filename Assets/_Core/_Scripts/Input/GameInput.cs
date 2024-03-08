using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    
    public static GameInput Instance { get; private set; }
    
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
        Interact_Gamepad,
        InteractAlternative_Gamepad,
        Pause_Gamepad
    }
    
    PlayerInputActions _playerInputActions;

    const string PLAYER_PREFS_BINDINGS = "InputBindings";
    
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
        
        _playerInputActions = new PlayerInputActions();
        
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Interact.performed += Interact_performed;
        _playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        _playerInputActions.Player.Pause.performed += Pause_performed;
    }

    void OnDestroy()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player.Interact.performed -= Interact_performed;
        _playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        _playerInputActions.Player.Pause.performed -= Pause_performed;
        
        _playerInputActions.Dispose();
    }

    void Interact_performed(InputAction.CallbackContext context)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    void InteractAlternate_performed(InputAction.CallbackContext context)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    void Pause_performed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        return binding switch
        {
            Binding.MoveUp => _playerInputActions.Player.Move.bindings[1].ToDisplayString(),
            Binding.MoveDown => _playerInputActions.Player.Move.bindings[2].ToDisplayString(),
            Binding.MoveLeft => _playerInputActions.Player.Move.bindings[3].ToDisplayString(),
            Binding.MoveRight => _playerInputActions.Player.Move.bindings[4].ToDisplayString(),
            Binding.Interact => _playerInputActions.Player.Interact.bindings[0].ToDisplayString(),
            Binding.InteractAlternate => _playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString(),
            Binding.Pause => _playerInputActions.Player.Pause.bindings[0].ToDisplayString(),
        };
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputActions.Player.Disable();
        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.MoveUp:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }
        
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                _playerInputActions.Player.Enable();
                onActionRebound();
                
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
}
