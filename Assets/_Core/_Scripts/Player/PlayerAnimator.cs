using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{

    const string IS_WALKING = "IsWalking";
    static readonly int IsWalking = Animator.StringToHash(IS_WALKING);

    [SerializeField] Player player;

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        _animator.SetBool(IsWalking, player.IsWalking);
    }
}
