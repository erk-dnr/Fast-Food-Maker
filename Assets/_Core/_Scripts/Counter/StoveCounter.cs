using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] BurningRecipeSO[] burningRecipeSOArray;
    [SerializeField] float burnWarningThreshold = 0.5f;

    NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0f);
    NetworkVariable<float> _burningTimer = new NetworkVariable<float>(0f);
    FryingRecipeSO _fryingRecipeSO;
    BurningRecipeSO _burningRecipeSO;

    public float BurnWarningThreshold => burnWarningThreshold;

    public override void OnNetworkSpawn()
    {
        _fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        _burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }
    public override void OnNetworkDespawn()
    {
        _fryingTimer.OnValueChanged -= FryingTimer_OnValueChanged;
        _burningTimer.OnValueChanged -= BurningTimer_OnValueChanged;
        _state.OnValueChanged -= State_OnValueChanged;
    }

    void Update()
    {
        if (!IsServer)
            return;
        
        if (HasKitchenObjectAttached)
        {
            switch (_state.Value)
            {
                case State.Idle:
                    break;
            
                case State.Frying:
                    _fryingTimer.Value += Time.deltaTime;
                    
                    if (_fryingTimer.Value > _fryingRecipeSO.fryingTimerMax)
                    {
                        // fried
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);

                        _state.Value = State.Fried;
                        _burningTimer.Value = 0f;
                        
                        int index =
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO());
                        
                        SetBurningRecipeSOClientRpc(index);
                    }
                    break;
            
                case State.Fried:
                    _burningTimer.Value += Time.deltaTime;
                    
                    if (_burningTimer.Value > _burningRecipeSO.burningTimerMax)
                    {
                        // burned
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_burningRecipeSO.output, this);

                        _state.Value = State.Burned;
                    }
                    break;
            
                case State.Burned:
                    break;
            }
        }
    }

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

                    int index =
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO());
                    PlaceObjectOnCounterServerRpc(index);
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
                        GetKitchenObject().DestroySelf();
 
                        _state.Value = State.Idle;
                    }
                }
            }
            else
            {
                // player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetStateIdleServerRpc()
    {
        _state.Value = State.Idle;
    }

    public bool IsFried()
    {
        return _state.Value == State.Fried;
    }
    
    void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = _fryingRecipeSO != null 
            ? _fryingRecipeSO.fryingTimerMax 
            : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = _fryingTimer.Value / fryingTimerMax
        });
    }
    
    void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = _burningRecipeSO != null 
            ? _burningRecipeSO.burningTimerMax 
            : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = _burningTimer.Value / burningTimerMax
        });
    }
    
    void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = _state.Value
        });

        if (_state.Value == State.Burned || _state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void PlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        _state.Value = State.Frying;
        _fryingTimer.Value = 0f;
        
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO =
            KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO =
            KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        
        return fryingRecipeSO != null;
    }

    KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        return null;
    }

    FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }

        return null;
    }
    
    BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }

        return null;
    }
}
