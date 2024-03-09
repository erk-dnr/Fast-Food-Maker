using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    
    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    [SerializeField] private GameInput gameInput;

    bool _isWalking;
    Vector3 _lastInteractDirection;
    BaseCounter _selectedCounter;
    KitchenObject _kitchenObject;

    public bool IsWalking => _isWalking;
    public bool HasPlate => _kitchenObject.TryGetPlate(out PlateKitchenObject plate);

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one player instance!");
        }
        Instance = this;
    }

    void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }
    
    void Update()
    {
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
    

    void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        // make movement frane rate independent
        Vector3 moveVector = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = movementSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
            playerRadius, moveVector, moveDistance);

        if (!canMove)
        {
            // cannot move towards moveVector
            
            // attempt only X movement
            Vector3 moveVectorX = new Vector3(moveVector.x, 0f, 0f).normalized;
            canMove = Mathf.Abs(moveVector.x) > 0.5f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                playerRadius, moveVectorX, moveDistance);

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
                canMove = Mathf.Abs(moveVector.z) > 0.5f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                    playerRadius, moveVectorZ, moveDistance);

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
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

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
}