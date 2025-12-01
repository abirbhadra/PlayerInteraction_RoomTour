using UnityEngine;

public class PickupAndDrop : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public float holdDistance = 1.5f;        // distance in front of camera
    public LayerMask grabbableLayer;

    private Rigidbody heldRB;
    private Collider heldCollider;

    void Update()
    {
        // LEFT CLICK = Pick or Drop
        if (Input.GetMouseButtonDown(0))
        {
            if (heldRB == null)
                TryPickup();
            else
                Drop();
        }

        // While holding, keep object in front of camera
        if (heldRB != null)
        {
            Vector3 targetPos = transform.position + transform.forward * holdDistance;
            heldRB.MovePosition(targetPos);
            // If you also want it to face forward:
            // heldRB.MoveRotation(transform.rotation);
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, grabbableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb == null) return;

            heldRB = rb;
            heldCollider = hit.collider;

            // Stop physics while holding
            heldRB.useGravity = false;
            heldRB.isKinematic = true;

            // Disable collisions so it doesn't push the player back
            if (heldCollider != null)
                heldCollider.enabled = false;
        }
    }

    void Drop()
    {
        if (heldRB == null) return;

        // Restore physics
        heldRB.useGravity = true;
        heldRB.isKinematic = false;

        // Re-enable collider so it can collide with world again
        if (heldCollider != null)
            heldCollider.enabled = true;

        heldRB = null;
        heldCollider = null;
    }
}
