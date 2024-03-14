using UnityEngine;

public class PlayerVisual : MonoBehaviour
{

    [SerializeField] MeshRenderer headMeshRenderer;
    [SerializeField] MeshRenderer bodyMeshRenderer;

    Material _material;

    void Awake()
    {
        // important to clone the material
        _material = new Material(headMeshRenderer.material);
        headMeshRenderer.material = _material;
        bodyMeshRenderer.material = _material;
    }

    public void SetPlayerColor(Color color)
    {
        _material.color = color;
    }
}
