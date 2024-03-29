using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;

    [SerializeField] CuttingRecipeSO[] cuttingRecipeSOArray;

    int _cuttingProgress;
    
    // selecting and dropping items
    public override void Interact(Player player)
    {
        // no kitchen object on the counter
        if (!HasKitchenObjectAttached)
        {
            if (player.HasKitchenObjectAttached)
            {
                // player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    PlaceObjectOnCounterServerRpc();


                }
                // else: item cannot be sliced
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
            }
            else
            {
                // player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    // use cutting counter / slice items
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObjectAttached && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // There is a KitchenObject on the counter AND it can be cut (not already done)
            CutObjectServerRpc();
            CheckCuttingProgressDoneServerRpc();
        }
    }

    bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        
        return cuttingRecipeSO != null;
    }

    KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }

        return null;
    }
    
    [ServerRpc(RequireOwnership = false)]
    void CutObjectServerRpc()
    {
        if (HasKitchenObjectAttached && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectClientRpc();
        }
    }

    [ClientRpc]
    void CutObjectClientRpc()
    {
        _cuttingProgress++;
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
            
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)_cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });
            
        if (_cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            KitchenObjectSO outputKitchenObjectSO = cuttingRecipeSO.output;
            // Destroy uncutted KitchenObject
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            // Spawn sliced KitchenObject of same type
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void CheckCuttingProgressDoneServerRpc()
    {
        if (HasKitchenObjectAttached && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            if (_cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlaceObjectOnCounterServerRpc()
    {
        PlaceObjectOnCounterClientRpc();
    }
    
    [ClientRpc]
    void PlaceObjectOnCounterClientRpc()
    {
        _cuttingProgress = 0;
                    
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }
}
