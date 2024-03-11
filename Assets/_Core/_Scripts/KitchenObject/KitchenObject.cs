using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(FollowTransform))]
public class KitchenObject : NetworkBehaviour
{

    [SerializeField] KitchenObjectSO kitchenObjectSO;

    IKitchenObjectParent _kitchenObjectParent;
    FollowTransform _followTransform;

    protected virtual void Awake()
    {
        _followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    void SetKitchenObjectParentServerRpc(NetworkObjectReference parentNetworkReference)
    {
        SetKitchenObjectParentClientRpc(parentNetworkReference);
    }

    [ClientRpc]
    void SetKitchenObjectParentClientRpc(NetworkObjectReference parentNetworkReference)
    {
        parentNetworkReference.TryGet(out NetworkObject parentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = parentNetworkObject.GetComponent<IKitchenObjectParent>();
        
        if (_kitchenObjectParent != null)
        { 
            _kitchenObjectParent.ClearKitchenObject();
        }

        _kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObjectAttached)
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject");
        }
        
        kitchenObjectParent.SetKitchenObject(this);
        
        _followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return _kitchenObjectParent;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectParent()
    {
        _kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    } 

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent parent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, parent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
    
}
