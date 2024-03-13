using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CharacterColorSelectSingleUI : MonoBehaviour
{

    [SerializeField] int colorId;
    [SerializeField] GameObject selectedGameObject;

    Image _image;
    Button _button;

    void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        
        _button.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        
        _image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        
        UpdateIsSelected();
    }

    void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }

    void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    void UpdateIsSelected()
    {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }
}
