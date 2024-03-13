using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] int playerIndex;
    [SerializeField] GameObject readyGameObject;
    [SerializeField] Button kickButton;
    [SerializeField] PlayerVisual playerVisual;

    void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        
        // only show Kick Button for the server
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        
        UpdatePlayer();
    }

    void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
    }

    void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    
    void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
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
