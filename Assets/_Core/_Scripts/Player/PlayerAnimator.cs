using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{

    const string IS_WALKING = "IsWalking";

    [SerializeField] Player player;

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        _animator.SetBool(IS_WALKING, player.IsWalking);
    }
}
