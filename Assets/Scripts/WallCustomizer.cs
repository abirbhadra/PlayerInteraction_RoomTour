using UnityEngine;

public class WallCustomizer : MonoBehaviour
{
    public Material[] materials;   // array for color options

    private WallSelectable currentWall;

    public void SetCurrentWall(WallSelectable wall)
    {
        currentWall = wall;
        Debug.Log("Selected wall: " + wall.gameObject.name);
    }

    public void ApplyMaterial(int index)
    {
        if (currentWall == null) return;
        if (index < 0 || index >= materials.Length) return;

        currentWall.meshRenderer.material = materials[index];
    }
}
