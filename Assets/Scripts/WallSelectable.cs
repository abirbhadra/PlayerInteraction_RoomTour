using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WallSelectable : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}

