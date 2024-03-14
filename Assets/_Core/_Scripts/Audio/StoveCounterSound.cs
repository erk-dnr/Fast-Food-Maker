using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StoveCounterSound : MonoBehaviour
{

    [SerializeField] StoveCounter stoveCounter;
    [SerializeField] float warningSoundTimerMax = 0.2f;
    
    AudioSource _audioSource;
    float _warningSoundTimer;
    bool _playWarningSound;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _warningSoundTimer = warningSoundTimerMax;
    }

    void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    void OnDestroy()
    {
        stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChanged -= StoveCounter_OnProgressChanged;
    }
    
    void Update()
    {
        if (!_playWarningSound)
            return;
        
        _warningSoundTimer -= Time.deltaTime;
        if (_warningSoundTimer <= 0f)
        {
            _warningSoundTimer = warningSoundTimerMax;
            
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
        }
    }

    void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSizzlingSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSizzlingSound)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Pause();
        }
    }
    
    void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        _playWarningSound = stoveCounter.IsFried() && e.progressNormalized >= stoveCounter.BurnWarningThreshold;
    }
}
