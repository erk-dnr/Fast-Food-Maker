using System;
using System.Collections.Generic;
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
        
        _kitchenObjectSOList.Add(kitchenObjectSO);
        
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            addedKitchenObjectSO = kitchenObjectSO
        });
        return true;
    }
}
