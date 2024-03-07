using UnityEngine;

[CreateAssetMenu(fileName = "New BurningRecipeSO", menuName = "SO/Burning Recipe")]
public class BurningRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;
    
}
