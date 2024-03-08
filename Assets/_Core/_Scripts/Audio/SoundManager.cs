using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioClipRefsSO audioClipRefsSO;

    float _volume;

    const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

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

        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    void Start()
    {
        DeliveryManager.Instance.OnRecipeSucceded += DeliveryManager_OnRecipeSucceded;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeSucceded -= DeliveryManager_OnRecipeSucceded;
        DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut -= CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething -= Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere -= BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed;
    }

    void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    void BaseCounter_OnAnyObjectPlacedHere(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.opjectDrop, baseCounter.transform.position);
    }

    void Player_OnPickedSomething(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
    }

    void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliveryFail, DeliveryCounter.Instance.transform.position);
    }

    void DeliveryManager_OnRecipeSucceded(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
    }
    
    void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, _volume * volumeMultiplier);
    }
    
    void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumeMultiplier);
    }

    public void PlayFootstepSound(Vector3 position, float volumeMultiplier)
    {
        PlaySound(audioClipRefsSO.footstep, position, volumeMultiplier);
    }

    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefsSO.warning, Vector3.zero);
    }

    public void ChangeVolume(float increment)
    {
        _volume += increment;
        if (_volume >= 1f + increment)
        {
            _volume = 0f;
        }
        
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return _volume;
    }
}
