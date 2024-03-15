using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GameStartCountdownUI : MonoBehaviour
{

    const string NUMBER_POPUP = "NumberPopup";
    static readonly int NumberPopup = Animator.StringToHash(NUMBER_POPUP);

    [SerializeField] TextMeshProUGUI countdownText;

    Animator _animator;

    int _previousCountdownNumber;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    void Update()
    {
        int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownTimer());
        countdownText.text = countdownNumber.ToString();

        if (_previousCountdownNumber != countdownNumber)
        {
            _previousCountdownNumber = countdownNumber;
            _animator.SetTrigger(NumberPopup);
            SoundManager.Instance.PlayCountdownSound();
        }
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownActive)
        {
            Show();
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
