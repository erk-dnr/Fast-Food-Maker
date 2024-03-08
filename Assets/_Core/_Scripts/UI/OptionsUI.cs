using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    [Header("Volume Controls")]
    [SerializeField] Button soundEffectsButton;
    [SerializeField] Button musicButton;
    [Tooltip("0.05 would mean: Increase volume by 5% per click")]
    [SerializeField] float volumeIncrementSteps = 0.1f;

    [Header("Key Binding Controls")] 
    [SerializeField] Button moveUpButton;
    [SerializeField] Button moveDownButton;
    [SerializeField] Button moveLeftButton;
    [SerializeField] Button moveRightButton;
    [SerializeField] Button interactButton;
    [SerializeField] Button interactAltButton;
    [SerializeField] Button pauseButton;
    [SerializeField] Button interactGPButton;
    [SerializeField] Button interactAltGPButton;
    [SerializeField] Button pauseGPButton;
    [SerializeField] Transform pressToRebindTransform;
    
    [Space]
    [SerializeField] Button closeButton;

    Action _onCloseButtonAction;

    public float VolumeScale => 1f / volumeIncrementSteps;

    void Awake()
    {
        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume(volumeIncrementSteps);
            UpdateVisual();
        });
        
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume(volumeIncrementSteps);
            UpdateVisual();
        });
        
        closeButton.onClick.AddListener(() =>
        {
            Hide();
            _onCloseButtonAction();
        });
        
        moveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp); });
        moveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown); });
        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight); });
        interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        interactAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlternate); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        interactGPButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Interact); });
        interactAltGPButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_InteractAlternate); });
        pauseGPButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Pause); });
    }

    void Start()
    {
        GameManager.Instance.OnGameResumed += GameManager_OnGameResumed;
        
        HidePressToRebindScreen();
        Hide();
        UpdateVisual();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGameResumed -= GameManager_OnGameResumed;
    }

    void GameManager_OnGameResumed(object sender, EventArgs e)
    {
        Hide();
    }

    void UpdateVisual()
    {
        soundEffectsButton.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"Sound Effects: {Mathf.RoundToInt(SoundManager.Instance.GetVolume() * VolumeScale)}";
        musicButton.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"Music: {Mathf.RoundToInt(MusicManager.Instance.GetVolume() * VolumeScale)}";

        moveUpButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDownButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        interactGPButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        interactAltGPButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        pauseGPButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);

    }

    void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindScreen();
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindScreen();
            UpdateVisual();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Action onCloseButtonAction)
    {
        _onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);
        
        closeButton.Select();
    }
    
    public void HidePressToRebindScreen()
    {
        pressToRebindTransform.gameObject.SetActive(false);
    }

    public void ShowPressToRebindScreen()
    {
        pressToRebindTransform.gameObject.SetActive(true);
    }
}
