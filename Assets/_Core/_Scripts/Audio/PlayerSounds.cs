using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerSounds : MonoBehaviour
{

    [SerializeField] float footstepTimerMax = 0.1f;
    [SerializeField] float footstepVolume = 1;

    Player _player;
    float _footstepTimer;

    void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _footstepTimer -= Time.deltaTime;
        if (_footstepTimer <= 0f)
        {
            ResetFootstepTimer();

            if (_player.IsWalking)
            {
                SoundManager.Instance.PlayFootstepSound(_player.transform.position, footstepVolume);
            }
        }
    }

    void ResetFootstepTimer()
    {
        _footstepTimer = footstepTimerMax;
    }
}
