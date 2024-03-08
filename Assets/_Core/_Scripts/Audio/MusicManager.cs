using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    
    public static MusicManager Instance { get; private set; }

    AudioSource _audioSource;
    float _volume;

    const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    
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

        _audioSource = GetComponent<AudioSource>();

        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);
        _audioSource.volume = _volume;
    }
    
    public void ChangeVolume(float increment)
    {
        _volume += increment;
        Debug.Log(_volume);
        if (_volume >= 1f + increment)
        {
            _volume = 0f;
        }

        _audioSource.volume = _volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return _volume;
    }
}
