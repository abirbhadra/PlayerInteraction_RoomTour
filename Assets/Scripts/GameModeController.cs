using UnityEngine;

public enum GameMode
{
    Play,
    EditorTopDown,
    EditorViewpoint
}

public class GameModeController : MonoBehaviour
{
    [Header("Play Mode")]
    public GameObject playerRoot;        // parent with CC + player camera
    public Camera playerCamera;

    [Header("Editor Cameras")]
    public Camera editorEnvCamera;       // Base camera (environment)
    public Camera editorHotspotCamera;   // Overlay camera (hotspots)

    [Header("Editor Logic")]
    public EditorCameraController editorController;

    [Header("UI References")]
    public Canvas editorCanvas;          //Canvas
    public GameObject wallColorPanel;    // wallColorPanel (buttons + Tab text)
    

    public GameMode CurrentMode { get; private set; }

    void Start()
    {
        SetMode(GameMode.Play);
    }

    void Update()
    {
        // E toggles Play - EditorTopDown
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (CurrentMode == GameMode.Play)
                SetMode(GameMode.EditorTopDown);
            else
                SetMode(GameMode.Play);
        }

        // Tab: viewpoint -> topdown
        if (CurrentMode == GameMode.EditorViewpoint &&
            Input.GetKeyDown(KeyCode.Tab))
        {
            SetMode(GameMode.EditorTopDown);
        }
    }

    public void SetMode(GameMode mode)
    {
        CurrentMode = mode;

        bool inEditor = (mode == GameMode.EditorTopDown ||
                         mode == GameMode.EditorViewpoint);
      

        if (playerRoot != null)
            playerRoot.SetActive(!inEditor);

        if (playerCamera != null)
            playerCamera.enabled = !inEditor;

        if (editorEnvCamera != null)
            editorEnvCamera.enabled = inEditor;

        if (editorHotspotCamera != null)
            editorHotspotCamera.enabled = inEditor;

        
        if (wallColorPanel != null)
            wallColorPanel.SetActive(mode == GameMode.EditorViewpoint);

        

        

        if (mode == GameMode.Play)
        {
            if (editorController != null)
                editorController.enabled = false;

            // Play mode = locked FPS cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (mode == GameMode.EditorTopDown)
        {
            if (editorController != null)
            {
                editorController.enabled = true;
                editorController.GoToTopDown();
            }

            // Editor = free visible mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (mode == GameMode.EditorViewpoint)
        {
            if (editorController != null)
                editorController.EnterViewpointMode();

            // Editor = free visible mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Called by EditorCameraController when slerp finishes
    public void OnViewpointReached()
    {
        SetMode(GameMode.EditorViewpoint);
    }
}
