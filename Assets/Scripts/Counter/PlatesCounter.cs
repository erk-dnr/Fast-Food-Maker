using System;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    
    [SerializeField] KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] float spawnPlateTimerMax = 4f;
    [SerializeField] int platesSpawnedAmountMax = 4;
    [SerializeField] bool spawnFirstPlateInstantly;

    float _spawnPlateTimer;
    int _platesSpawnedAmount;
    bool _runPlateSpawnTimer = true;

    void Start()
    {
        if (spawnFirstPlateInstantly)
        {
            _platesSpawnedAmount++;
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }
    }

    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying)
            return;
        
        // timer paused
        if (!_runPlateSpawnTimer)
            return;

        _spawnPlateTimer += Time.deltaTime;
        
        if (_spawnPlateTimer > spawnPlateTimerMax)
        {
            _spawnPlateTimer = 0f;

            if (_platesSpawnedAmount < platesSpawnedAmountMax)
            {
                // can spawn new plate
                _platesSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
                if (_platesSpawnedAmount == platesSpawnedAmountMax)
                {
                    // maximum amount of plates on the counter
                    _runPlateSpawnTimer = false;
                }
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObjectAttached)
        {
            // player is empty handed
            if (_platesSpawnedAmount > 0)
            {
                // there is at least one plate
                _platesSpawnedAmount--;
                // new plate can be spawned after time, so restart timer
                _runPlateSpawnTimer = true;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
