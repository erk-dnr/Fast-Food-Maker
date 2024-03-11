using System;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole Instance { get; private set; }

    [SerializeField] RectTransform displayRect;
    [SerializeField] TextMeshProUGUI displayText;

    float _initialHeight;

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
        _initialHeight = displayRect.anchoredPosition.y;
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

    public void ChangeDisplayPosition(float newYPosition)
    {
        displayRect.anchoredPosition = new Vector2(displayRect.anchoredPosition.x, _initialHeight + newYPosition);
    }

    void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            Log(condition);
        }

        if (type == LogType.Exception || type == LogType.Error)
        {
            LogError(condition);
        }

        if (type == LogType.Warning)
        {
            LogWarning(condition);
        }

        if (type == LogType.Assert)
        {
            LogAssertion(condition);
        }
    }

    void Log(string newLog)
    {
        displayText.text = $"[{GetTime()}] - {newLog}\n{displayText.text}";
    }

    void LogWarning(string newLog)
    {
        displayText.text = $"[{GetTime()}] - <color=orange>{newLog}</color>\n{displayText.text}";
    }
    
    void LogError(string newLog)
    {
        displayText.text = $"[{GetTime()}] - <color=red>{newLog}</color>\n{displayText.text}";
    }

    void LogAssertion(string newLog)
    {
        displayText.text = $"[{GetTime()}] - <color=blue>{newLog}</color>\n{displayText.text}";
    }
    
    string GetTime() {
        DateTime time = DateTime.Now;
        return time.ToString("HH:mm:ss");
    }
}
