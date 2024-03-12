using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO addedKitchenObjectSO;
    }

    [SerializeField] List<KitchenObjectSO> validKitchenObjectSOList;

    List<KitchenObjectSO> _kitchenObjectSOList;

    public List<KitchenObjectSO> KitchenObjectSOList => _kitchenObjectSOList;
    protected override void Awake()
    {
        base.Awake();
        _kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // not a valid ingredient
            return false;
        }
        
        if (_kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // already has this type
            return false;
        }

        int index = KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO);
        AddIngredientServerRpc(index);
        
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO =
            KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _kitchenObjectSOList.Add(kitchenObjectSO);
        
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            addedKitchenObjectSO = kitchenObjectSO
        });
    }
}
