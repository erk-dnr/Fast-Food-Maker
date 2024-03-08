using System;
using UnityEngine;

[SelectionBase]
public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{

    public static event EventHandler OnAnyObjectPlacedHere;
    
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject _kitchenObject;

    public virtual void Interact(Player player)
    {
        Debug.Log("BaseCounter.Interact();");
    }
    
    public virtual void InteractAlternate(Player player)
    {
        // Debug.Log("BaseCounter.InteractAlternate();");
    }

    public bool HasKitchenObjectAttached => _kitchenObject != null;
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;

        if (_kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public virtual Transform GetCounterTopPoint()
    {
        return counterTopPoint;
    }
}
