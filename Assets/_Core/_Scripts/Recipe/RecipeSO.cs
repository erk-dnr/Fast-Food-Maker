using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeSO", menuName = "SO/Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
}
