using System;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] KitchenObjectSO kitchenObjectSO;
    
    
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObjectAttached)
        {
            // player is not carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
        // player is carrying something
        else
        {
            // check for plate
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                plateKitchenObject.TryAddIngredient(kitchenObjectSO);
            }
        }
    }
}
