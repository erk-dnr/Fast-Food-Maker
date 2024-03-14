using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{

    [SerializeField] Button closeButton;
    [SerializeField] Button createPrivateButton;
    [SerializeField] Button createPublicButton;
    [SerializeField] TMP_InputField lobbyNameInput;

    void Awake()
    {
        createPublicButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInput.text, false);
        });
        
        createPrivateButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInput.text, true);
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
