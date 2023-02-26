using UnityEngine;

public class GenshinCameraController : MonoBehaviour
{
    // The target to follow (the player)
    public Transform target;

    // The maximum distance the camera can be from the target
    public float maxDistance = 10f;

    // The minimum distance the camera can be from the target
    public float minDistance = 2f;

    // The speed at which the camera moves
    public float moveSpeed = 10f;

    // The speed at which the camera rotates
    public float rotateSpeed = 5f;

    // The layer mask to use for obstacle detection
    public LayerMask obstacleMask;

    // The maximum angle the camera can be above the target
    public float maxAngle = 80f;

    // The minimum angle the camera can be below the target
    public float minAngle = -20f;

    // The distance at which the player becomes translucent
    public float playerTransparencyDistance = 1f;

    // The material to use for the player when translucent
    public Material translucentMaterial;

    // The original material of the player
    private Material originalMaterial;

    // The current distance between the camera and the target
    private float distance = 10f;

    // The current angle of the camera around the target
    private float angle = 45f;

    // The current angle of the camera above the target
    private float height = 5f;

    // The speed at which the camera zooms in and out
    public float zoomSpeed = 7f;

    private void Start()
    {
        // Cache the original material of the player
        originalMaterial = target.GetComponent<Renderer>().material;
    }

    private void LateUpdate()
    {
        // Get the mouse input for rotation
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;

        // Rotate the camera around the target
        angle += horizontal;
        height -= vertical;

        // Clamp the height to the allowable range
        height = Mathf.Clamp(height, minAngle, maxAngle);

        // Calculate the position and rotation of the camera
        Quaternion rotation = Quaternion.Euler(height, angle, 0f);
        Vector3 position = target.position - rotation * Vector3.forward * distance;

        // Check for obstacles between the camera and the target
        RaycastHit hit;
        if (Physics.Linecast(target.position, position, out hit, obstacleMask))
        {
            // Move the camera in front of the obstacle
            position = hit.point + hit.normal * 0.1f;
        }

        // Move the camera towards the target
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * moveSpeed);

        // Look at the target
        transform.LookAt(target);

        // Check the distance between the camera and the target
        float playerDistance = Vector3.Distance(transform.position, target.position);
        if (playerDistance < playerTransparencyDistance)
        {
            // Make the player translucent
            target.GetComponent<Renderer>().material = translucentMaterial;
        }
        else
        {
            // Restore the original material of the player
            target.GetComponent<Renderer>().material = originalMaterial;
        }
    }

    private void FixedUpdate()
    {
        // Get the input for zooming
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        // Zoom the camera in or out
        distance -= zoom;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }
}