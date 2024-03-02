using UnityEngine;

[CreateAssetMenu(fileName = "New CuttingRecipeSO", menuName = "SO/Cutting Recipe")]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}
