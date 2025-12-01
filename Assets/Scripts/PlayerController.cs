using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform cameraTransform;  // assign your PlayerCamera here

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float verticalLookLimit = 80f;   // max up/down angle

    float verticalVelocity;
    float cameraPitch = 0f;                 // current up/down rotation

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        // mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // rotate body left/right
        transform.Rotate(Vector3.up * mouseX);

        // rotate camera up/down (clamped)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        // WASD input
        float x = Input.GetAxisRaw("Horizontal");   // A/D
        float z = Input.GetAxisRaw("Vertical");     // W/S

        Vector3 move = new Vector3(x, 0f, z);
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // convert local → world based on where the player is facing
        move = transform.TransformDirection(move) * moveSpeed;

        // gravity & jump
        if (controller.isGrounded)
        {
            verticalVelocity = -1f; // small push down to stay grounded

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}
