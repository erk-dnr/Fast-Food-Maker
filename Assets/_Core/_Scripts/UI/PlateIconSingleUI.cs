using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{

    [SerializeField] Image iconImage;

    public void SetKitchenObjectSprite(KitchenObjectSO kitchenObjectSO)
    {
        iconImage.sprite = kitchenObjectSO.sprite;
    }
}
