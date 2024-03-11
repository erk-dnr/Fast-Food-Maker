using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour
{
    
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSucceded;
    public event EventHandler OnRecipeFailed;
    
    
    [SerializeField] RecipeListSO recipeListSO;
    [SerializeField] float spawnRecipeTimerMax = 4f;
    [SerializeField] int waitingRecipesMax = 4;

    List<RecipeSO> _waitingRecipes;
    float _spawnRecipeTimer;
    int _successfulRecipesAmount;
    
    public List<RecipeSO> WaitingRecipes => _waitingRecipes;
    public int SuccessfulRecipesAmount => _successfulRecipesAmount;

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
        
        _waitingRecipes = new List<RecipeSO>();
    }

    void Update()
    {
        // only the server spawns the recipes
        if (!IsServer) return;
        
        if (!GameManager.Instance.IsGamePlaying)
            return;
        
        _spawnRecipeTimer -= Time.deltaTime;
        if (_spawnRecipeTimer <= 0f)
        {
            ResetSpawnRecipeTimer();

            if (_waitingRecipes.Count < waitingRecipesMax)
            {
                // create new recipe
                int waitingRecipesIndex = Random.Range(0, recipeListSO.recipeList.Count);
                
                SpawnNewWaitingRecipeClientRPC(waitingRecipesIndex);
            }
        }
    }

    // we send the index of the list, instead of the RecipeSO directly,
    // because int is serializable
    [ClientRpc]
    void SpawnNewWaitingRecipeClientRPC(int waitingRecipesIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeList[waitingRecipesIndex];
        _waitingRecipes.Add(waitingRecipeSO);
        
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < _waitingRecipes.Count; i++)
        {
            RecipeSO waitingRecipeSO = _waitingRecipes[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.KitchenObjectSOList.Count)
            {
                // has the same number of ingredients
                bool plateContentMatchesRecipe = true;
                
                foreach (var recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    // cycling through all ingredients in the recipe
                    foreach (var plateKitchenObjectSO in plateKitchenObject.KitchenObjectSOList)
                    {
                        // cycling through all ingredients on the plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // ingredient does match
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        // the ingredient was not found on the plate
                        plateContentMatchesRecipe = false;
                    }
                }

                if (plateContentMatchesRecipe)
                {
                    // player delivereed correct recipe
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        
        // no matches found -> player did not deliver a correct recipe
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void DeliverCorrectRecipeServerRpc(int waitingRecipesIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipesIndex);
    }
    
    [ClientRpc]
    void DeliverCorrectRecipeClientRpc(int waitingRecipesIndex)
    {
        _successfulRecipesAmount++;
        _waitingRecipes.RemoveAt(waitingRecipesIndex);
                    
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSucceded?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }
    
    [ClientRpc]
    void DeliverIncorrectRecipeClientRpc()
    {
        Debug.Log("Player did not deliver correct recipe");
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    void ResetSpawnRecipeTimer()
    {
        _spawnRecipeTimer = spawnRecipeTimerMax;
    }
}
