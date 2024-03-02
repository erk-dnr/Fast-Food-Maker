using UnityEngine;

public interface IKitchenObjectParent
{
    
    bool HasKitchenObjectAttached { get; }

    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();

    public void ClearKitchenObject();
}
