using UnityEngine;

public class Superliminal : MonoBehaviour
{
    [Header("Components")]
    public Transform target;            // The target object we picked up for scaling

    [Header("Parameters")]
    public LayerMask targetMask;        // The layer mask used to hit only potential targets with a raycast
    public LayerMask ignoreTargetMask;  // The layer mask used to ignore the player and target objects while raycasting
    public float offsetFactor;          // The offset amount for positioning the object so it doesn't clip into walls
    public float detectionAccuracy;

    float originalDistance;             // The original distance between the player camera and the target
    float originalScale;                // The original scale of the target objects prior to being resized
    Vector3 targetScale;                // The scale we want our object to be set to each frame

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleInput();
        ResizeTarget();
    }

    void HandleInput()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // If we do not currently have a target
            if (target == null)
            {
                // Fire a raycast with the layer mask that only hits potential targets
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))
                {
                    // Set our target variable to be the Transform object we hit with our raycast
                    target = hit.transform;

                    target.transform.parent = transform;//sync the rotation

                    // Disable physics for the object
                    target.GetComponent<Rigidbody>().isKinematic = true;

                    // Calculate the distance between the camera and the object
                    originalDistance = Vector3.Distance(transform.position, target.position);

                    // Save the original scale of the object into our originalScale Vector3 variabble
                    originalScale = target.localScale.x;

                    // Set our target scale to be the same as the original for the time being
                    targetScale = target.localScale;

                    target.gameObject.layer = LayerMask.NameToLayer("SelectedTarget");
                }
            }
            // If we DO have a target
            else
            {
                // Reactivate physics for the target object
                target.GetComponent<Rigidbody>().isKinematic = false;

                target.gameObject.layer = LayerMask.NameToLayer("Targetable");

                target.transform.parent = null;
                // Set our target variable to null
                target = null;

                offsetFactor = 0;
            }
        }
    }

    void ResizeTarget()
    {
        // If our target is null
        if (target == null)
        {
            // Return from this method, nothing to do here
            return;
        }

        // Cast a ray forward from the camera position, ignore the layer that is used to acquire targets
        // so we don't hit the attached target with our ray
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, ignoreTargetMask))
        {
            // Set the new position of the target by getting the hit point and moving it back a bit
            // depending on the scale and offset factor
            if (Vector3.Distance(transform.position, target.position) > 0)
            {
                target.position = hit.point - transform.forward * offsetFactor * targetScale.x;
            }

            if (target.GetComponent<ItemOverlap>().isOverlap == true)
            {
                offsetFactor += detectionAccuracy;
                target.position = hit.point - transform.forward * offsetFactor * targetScale.x;
            }

            // Calculate the current distance between the camera and the target object
            float currentDistance = Vector3.Distance(transform.position, target.position);

            // Calculate the ratio between the current distance and the original distance
            float s = currentDistance / originalDistance;

            // Set the scale Vector3 variable to be the ratio of the distances
            targetScale.x = targetScale.y = targetScale.z = s;

            // Set the scale for the target objectm, multiplied by the original scale
            target.localScale = targetScale * originalScale;
        }
    }
}
