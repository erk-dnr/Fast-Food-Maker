using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{

    [SerializeField] PlateKitchenObject plateKitchenObject;
    [SerializeField] Transform iconTemplate;

    void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }
    
    void OnDestroy()
    {
        plateKitchenObject.OnIngredientAdded -= PlateKitchenObject_OnIngredientAdded;
    }

    void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == iconTemplate)
                continue;
            Destroy(child.gameObject);
        }
        foreach (var kitchenObjectSO in plateKitchenObject.KitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSprite(kitchenObjectSO);
        }
    }
}
