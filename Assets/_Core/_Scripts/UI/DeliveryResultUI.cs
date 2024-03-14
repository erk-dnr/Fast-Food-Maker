using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class DeliveryResultUI : MonoBehaviour
{

    [SerializeField] Image backgroundImage;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Color successColor;
    [SerializeField] Color failColor;
    [SerializeField] Sprite successSprite;
    [SerializeField] Sprite failSprite;
    
    const string POPUP = "Popup";
    static readonly int Popup = Animator.StringToHash(POPUP);

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        DeliveryManager.Instance.OnRecipeSucceded += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        Hide();
    }

    void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeSucceded -= DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
    }

    void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        Show();
        _animator.SetTrigger(Popup);
        backgroundImage.color = successColor;
        icon.sprite = successSprite;
        message.text = "Delivery\nSuccess";
    }

    void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        Show();
        _animator.SetTrigger(Popup);
        backgroundImage.color = failColor;
        icon.sprite = failSprite;
        message.text = "Delivery\nFailed";
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
