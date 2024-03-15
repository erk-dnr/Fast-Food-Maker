using UnityEngine;

public class ClearCounter : BaseCounter
{

    public override void Interact(Player player)
    {
        // no kitchen object on the counter
        if (!HasKitchenObjectAttached)
        {
            if (player.HasKitchenObjectAttached)
            {
                // player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
                // player drops the plate
            }
            else
            {
                // player not carrying anything
                Debug.Log("no KitchenObject to interact with");
            }
        }
        // there is a kitchen object on the counter
        else
        {
            if (player.HasKitchenObjectAttached)
            {
                // player is carrying something

                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // added item to the player's plate was successful -> remove it from the counter
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    // player is not holding plate but something else (ingredient)
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            // added item to the counter's plate was successful -> remove it from the player
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else
            {
                // player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
