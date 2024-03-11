using System;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{

    public static event EventHandler OnAnyObjectTrashed; 
        
    public override void Interact(Player player)
    {
        if (player.HasKitchenObjectAttached)
        {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            
            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    
    [ClientRpc]
    void InteractLogicClientRpc()
    {
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}
