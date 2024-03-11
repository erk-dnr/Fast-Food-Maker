using System;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI keyMoveUpText;
    [SerializeField] TextMeshProUGUI keyMoveDownText;
    [SerializeField] TextMeshProUGUI keyMoveLeftText;
    [SerializeField] TextMeshProUGUI keyMoveRightText;
    [SerializeField] TextMeshProUGUI keyInteractText;
    [SerializeField] TextMeshProUGUI keyInteractAltText;
    [SerializeField] TextMeshProUGUI keyPauseText;
    [SerializeField] TextMeshProUGUI keyInteractGPText;
    [SerializeField] TextMeshProUGUI keyInteractAltGPText;
    [SerializeField] TextMeshProUGUI keyPauseGPText;
    
    
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        GameInput.Instance.OnBindingRebind += GameInput_OnBindRebind;
        
        UpdateVisual();
        // Show();
        Hide();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
        GameInput.Instance.OnBindingRebind -= GameInput_OnBindRebind;
    }
    
    void UpdateVisual()
    {
        keyMoveUpText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        keyMoveDownText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        keyMoveLeftText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        keyMoveRightText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        keyInteractText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyInteractAltText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        keyPauseText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        keyInteractGPText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        keyInteractAltGPText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        keyPauseGPText.text =
            GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);

    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownActive)
        {
            Hide();
        }
    }

    void GameInput_OnBindRebind(object sender, EventArgs e)
    {
        UpdateVisual();
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
