using System;
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

    State _state;
    float _fryingTimer;
    float _burningTimer;
    FryingRecipeSO _fryingRecipeSO;
    BurningRecipeSO _burningRecipeSO;

    public float BurnWarningThreshold => burnWarningThreshold;

    void Start()
    {
        _state = State.Idle;
        _fryingTimer = 0f;
    }

    void Update()
    {
        if (HasKitchenObjectAttached)
        {
            switch (_state)
            {
                case State.Idle:
                    break;
            
                case State.Frying:
                    _fryingTimer += Time.deltaTime;
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = _fryingTimer / _fryingRecipeSO.fryingTimerMax
                    });
                    
                    if (_fryingTimer > _fryingRecipeSO.fryingTimerMax)
                    {
                        // fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);

                        _state = State.Fried;
                        _burningTimer = 0f;
                        _burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = _state
                        });
                    }
                    break;
            
                case State.Fried:
                    _burningTimer += Time.deltaTime;
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = _burningTimer / _burningRecipeSO.burningTimerMax
                    });
                    
                    if (_burningTimer > _burningRecipeSO.burningTimerMax)
                    {
                        // burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_burningRecipeSO.output, this);

                        _state = State.Burned;
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = _state
                        });
                        
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
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
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    _state = State.Frying;
                    _fryingTimer = 0f;
                    
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = _state
                    });
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = _fryingTimer / _fryingRecipeSO.fryingTimerMax
                    });
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

                        _state = State.Idle;
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = _state
                        });
                        
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                // player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
                _state = State.Idle;
                
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = _state
                });
                
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    public bool IsFried()
    {
        return _state == State.Fried;
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
