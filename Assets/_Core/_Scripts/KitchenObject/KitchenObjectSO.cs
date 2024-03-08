using UnityEngine;

[CreateAssetMenu(fileName = "New KitchenObject", menuName = "SO/Kitchen Object")]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}