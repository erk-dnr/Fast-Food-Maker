using UnityEngine;

[CreateAssetMenu(fileName = "New FryingRecipeSO", menuName = "SO/Frying Recipe")]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;
}
