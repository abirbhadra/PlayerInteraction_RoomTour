using System.Collections;
using UnityEngine;

public class EditorCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform roomCenter;               
    public GameModeController gameMode;
    public WallCustomizer wallCustomizer;

    [Header("Top Down Settings")]
    public float initialOrbitX = 60f;         // starting vertical angle
    public float initialOrbitY = 45f;         // starting horizontal angle
    public float topDownDistance = 8f;        // starting distance from center
    public float orbitSpeed = 120f;           // mouse drag sensitivity
    public float zoomSpeed = 5f;              // scroll sensitivity
    public float minZoom = 3f;                // min distance
    public float maxZoom = 20f;               // max distance

    [Header("Viewpoint Settings")]
    public float slerpDuration = 1.0f;        // time to slerp to viewpoint
    public float lookSensitivity = 2f;        // mouse look sensitivity in viewpoint

    // state
    bool isTopDown = true;
    bool isSlerping = false;
    Transform currentViewpoint;

    // top-down orbit state
    float orbitX;
    float orbitY;
    float currentDistance;

    // viewpoint look state
    float yaw;
    float pitch;

    void Start()
    {
        GoToTopDown();
    }

    void Update()
    {
        if (isSlerping) return;

        if (isTopDown)
            HandleTopDown();
        else
            HandleViewpoint();
    }

    // ================= TOP-DOWN MODE =================

    public void GoToTopDown()
    {
        isTopDown = true;

        // initialize orbit + distance
        orbitX = initialOrbitX;
        orbitY = initialOrbitY;
        currentDistance = Mathf.Clamp(topDownDistance, minZoom, maxZoom);

        UpdateTopDownPosition();
    }

    void HandleTopDown()
    {
        // Free orbit with RIGHT mouse button
        if (Input.GetMouseButton(1))
        {
            // horizontal
            float mouseX = Input.GetAxis("Mouse X");
            // vertical (invert if you prefer)
            float mouseY = Input.GetAxis("Mouse Y");

            orbitY += mouseX * orbitSpeed * Time.deltaTime;
            orbitX -= mouseY * orbitSpeed * Time.deltaTime;

            // clamp vertical angle so we don't flip under the floor
            orbitX = Mathf.Clamp(orbitX, 10f, 85f);
        }

        // Zoom with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minZoom, maxZoom);
        }

        UpdateTopDownPosition();
    }

    void UpdateTopDownPosition()
    {
        if (roomCenter == null) return;

        Quaternion rot = Quaternion.Euler(orbitX, orbitY, 0f);
        Vector3 dir = rot * Vector3.forward;       // forward from camera towards center

        // place camera at center - dir * distance, then look at center
        transform.position = roomCenter.position - dir * currentDistance;
        transform.LookAt(roomCenter.position);
    }

    // ================= VIEWPOINT MODE =================

    public void MoveToViewpoint(Transform targetViewpoint)
    {
        if (isSlerping || targetViewpoint == null) return;
        StartCoroutine(SlerpToViewpoint(targetViewpoint));
    }

    IEnumerator SlerpToViewpoint(Transform target)
    {
        isSlerping = true;
        isTopDown = false;
        currentViewpoint = target;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slerpDuration;
            float lerpT = Mathf.Clamp01(t);

            transform.position = Vector3.Lerp(startPos, endPos, lerpT);
            transform.rotation = Quaternion.Slerp(startRot, endRot, lerpT);
            yield return null;
        }

        // initialize yaw/pitch from final rotation
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;

        isSlerping = false;

        // tell GameModeController that we reached a viewpoint
        if (gameMode != null)
            gameMode.OnViewpointReached();
    }

    public void EnterViewpointMode()
    {
        isTopDown = false;

        if (currentViewpoint != null)
        {
            transform.position = currentViewpoint.position;
            transform.rotation = currentViewpoint.rotation;

            Vector3 euler = transform.eulerAngles;
            yaw = euler.y;
            pitch = euler.x;
        }
    }

    void HandleViewpoint()
    {
        // Look around ONLY while holding right mouse
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // Left click to select wall in front of camera
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                WallSelectable wall = hit.collider.GetComponent<WallSelectable>();
                if (wall != null && wallCustomizer != null)
                {
                    wallCustomizer.SetCurrentWall(wall);
                }
            }
        }
    }
}
