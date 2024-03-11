using System;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{

    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] KitchenObjectListSO kitchenObjectList;
    

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
    
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent parent)
    {
        /*  This line stops an error when playing in the Unity Editor
         (key "KitchenGameMultiplayer" not found in dictionary... at SpawnKitchenObjectServerRpc)
         But it would be bad, because client interactions are not synchronized that way
        */
        // if (!IsServer) return;
        
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), parent.GetNetworkObject());
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnKitchenObjectServerRpc(int kitchenObjectListIndex, NetworkObjectReference parentNetworkReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectListIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
    
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        parentNetworkReference.TryGet(out NetworkObject parentNetworkObject);
        IKitchenObjectParent parent = parentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(parent);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkReference)
    {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        
        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkReference);
        
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkReference)
    {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        
        kitchenObject.ClearKitchenObjectParent();
    }


    int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectList.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        return kitchenObjectList.kitchenObjectSOList[index];
    }
}
