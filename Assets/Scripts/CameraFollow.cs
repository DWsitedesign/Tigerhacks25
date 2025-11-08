using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Vector3 offset;
    [SerializeField] public float smoothSpeed = 0.125f;
    [SerializeField] private LayerMask wallLayer;   // Assign “Wall” layer in Inspector
    [SerializeField] private float wallBuffer = 2f; // how far from wall to stop camera

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z; // Maintain original z position
        targetPosition.y = transform.position.y; // Maintain original z position

        Vector3 origin = target.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        // Debug.Log("Origin: " + origin + " TargetPos: " + targetPosition + " Direction: " + direction);
        Debug.DrawRay(origin, direction * wallBuffer, Color.red);

        // Check for wall obstruction
        if (Physics.Raycast(origin, direction, out RaycastHit hit, wallBuffer, wallLayer))
        {
            // Debug.Log("Camera obstructed by wall: " + hit.collider.name);
            return;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);

    }
    public void CheckWall()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z; // Maintain original z position
        targetPosition.y = transform.position.y; // Maintain original z position

        // Debug.Log("Origin: " + origin + " TargetPos: " + targetPosition + " Direction: " + direction);
        Debug.DrawRay(target.position, Vector3.left*wallBuffer, Color.red);
        Debug.DrawRay(target.position, Vector3.right*wallBuffer, Color.red);

        // Check for wall obstruction
        if (Physics.Raycast(target.position, Vector3.left, out RaycastHit hit, wallBuffer, wallLayer))
        {
            Debug.Log("Camera obstructed by wall L: " + hit.collider.name);
            targetPosition.x = hit.point.x + wallBuffer;
        }
        if (Physics.Raycast(target.position, Vector3.right, out hit, wallBuffer, wallLayer))
        {
            Debug.Log("Camera obstructed by wall R: " + hit.collider.name);
            targetPosition.x = hit.point.x - wallBuffer;
        }
        transform.position = targetPosition;
    }
}
