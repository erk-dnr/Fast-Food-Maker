using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField] float movementSpeed = 7f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] LayerMask countersLayerMask;
    [SerializeField] LayerMask collisionsLayerMask;
    [SerializeField] Transform kitchenObjectHoldPoint;
    [SerializeField] PlayerVisual playerVisual;
    [SerializeField] List<Vector3> spawnPositionList;

    // [SerializeField] private GameInput gameInput;

    bool _isWalking;
    Vector3 _lastInteractDirection;
    BaseCounter _selectedCounter;
    KitchenObject _kitchenObject;

    public bool IsWalking => _isWalking;
    public bool HasPlate => _kitchenObject.TryGetPlate(out _);

    void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        int playerIndex = KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId);
        transform.position = spawnPositionList[playerIndex];
        
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
        }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;
    }

    void Update()
    {
        if (!IsOwner) return;
        
        HandleMovement();
        HandleInteractions();
    }

    void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying)
            return;
        
        if (_selectedCounter != null)
        {
            _selectedCounter.Interact(this);
        }
    }
    
    void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying)
            return;
        
        if (_selectedCounter != null)
        {
            _selectedCounter.InteractAlternate(this);
        }
    }
    
    void NetworkManager_OnClientDisconnect(ulong clientId)
    {
        // destroyed attached object to a player that is disconnecting
        if (clientId == OwnerClientId && HasKitchenObjectAttached)
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }
    
    void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        
        // make movement frane rate independent
        Vector3 moveVector = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = movementSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveVector, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            // cannot move towards moveVector
            
            // attempt only X movement
            Vector3 moveVectorX = new Vector3(moveVector.x, 0f, 0f).normalized;
            canMove = Mathf.Abs(moveVector.x) > 0.5f && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveVectorX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                // can move only on the X
                moveVector = moveVectorX;
            }
            else
            {
                // cannot move only on the X
                
                //attempt only Z movement
                Vector3 moveVectorZ = new Vector3(0f, 0f, moveVector.z).normalized;
                canMove = Mathf.Abs(moveVector.z) > 0.5f && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveVectorZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    // can move only on the Z
                    moveVector = moveVectorZ;
                }
                else
                {
                    // cannot move in any direction
                }
            }
        }
        

        if (canMove)
        {
            transform.position += moveVector * moveDistance;
            // update isWalking boolean
            _isWalking = moveVector != Vector3.zero;

            // make player face the moving direction
            transform.forward = Vector3.Slerp(transform.forward, moveVector, Time.deltaTime * rotationSpeed);
        }
    }

    void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveVector = new Vector3(inputVector.x, 0f, inputVector.y);
        
        if (moveVector != Vector3.zero)
        {
            _lastInteractDirection = moveVector;
        }

        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, _lastInteractDirection, out RaycastHit hitObject, interactDistance, countersLayerMask))
        {
            if (hitObject.transform.TryGetComponent(out BaseCounter counter))
            {
                // has BaseCounter
                if (counter != _selectedCounter)
                {
                    SetSelectedCounter(counter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        { 
            SetSelectedCounter(null);
        }
    }

    void SetSelectedCounter(BaseCounter selectedCounter)
    {
        _selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public bool HasKitchenObjectAttached => _kitchenObject != null;
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;

        if (_kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
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

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}