using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StoveCounterSound : MonoBehaviour
{

    [SerializeField] StoveCounter stoveCounter;
    
    AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }
    
    void OnDestroy()
    {
        stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
    }

    void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSound)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Pause();
        }
    }
}
