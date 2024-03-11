using System;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    
    public static DeliveryCounter Instance { get; private set; }

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

    public override void Interact(Player player)
    {
        if (player.HasKitchenObjectAttached)
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // only accepts plates
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }
}
