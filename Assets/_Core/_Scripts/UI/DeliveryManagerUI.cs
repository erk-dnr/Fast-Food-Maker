using System;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{

    [SerializeField] Transform container;
    [SerializeField] Transform recipeTemplate;

    void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        
        UpdateVisual();
        Hide();
    }
    

    void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeSpawned -= DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted -= DeliveryManager_OnRecipeCompleted;
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }
    
    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying)
        {
            Show();
        }
    }
    
    void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }
    
    void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }
    
    void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate)
                continue;

            Destroy(child.gameObject);
        }

        if (DeliveryManager.Instance.WaitingRecipes.Count > 0)
        {
            foreach (var recipeSO in DeliveryManager.Instance.WaitingRecipes)
            {
                Transform recipeTransform = Instantiate(recipeTemplate, container);
                recipeTransform.gameObject.SetActive(true);
                recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeTemplate(recipeSO);
            }
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
