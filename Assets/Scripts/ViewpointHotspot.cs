using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ViewpointHotspot : MonoBehaviour
{
    public Transform viewpoint;                 // empty transform inside room
    public EditorCameraController editorCamera; 

    void OnMouseDown()
    {
        editorCamera.MoveToViewpoint(viewpoint);
    }
}
