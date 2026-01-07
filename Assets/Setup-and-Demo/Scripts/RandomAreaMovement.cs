using UnityEngine;

public class RandomAreaMovement : MonoBehaviour
{
    public Transform area;            // The cube area where the insect can move
    public float speed = 2f;          // Movement speed
    public float heightOffset = 0.5f; // Base height (minimum height)
    public float turnSpeed = 2f;      // How smoothly it rotates

    [Header("Vertical Flutter (Up/Down)")]
    public float flutterAmplitude = 0.3f;   // how high/low it moves (0.3 is good)
    public float flutterSpeed = 4f;         // how fast it flutters

    Vector3 targetPosition;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        MoveTowardsTarget();
        CheckIfReachedTarget();
    }

    void PickNewTarget()
    {
        if (area == null) return;

        Vector3 center = area.position;
        Vector3 size = area.localScale;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        // Base height only (no flutter here)
        float y = center.y + heightOffset;

        targetPosition = new Vector3(x, y, z);
    }

    void MoveTowardsTarget()
    {
        // 1. Move horizontally toward target
        Vector3 horizontalTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);

        // 2. Add flutter (up and down)
        float flutterY = Mathf.Sin(Time.time * flutterSpeed) * flutterAmplitude;
        transform.position = new Vector3(transform.position.x,
                                         targetPosition.y + flutterY,
                                         transform.position.z);

        // 3. Smooth rotation toward movement direction
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }
    }

    void CheckIfReachedTarget()
    {
        // When close enough, choose a new random point
        if (Vector3.Distance(new Vector3(transform.position.x, targetPosition.y, transform.position.z), targetPosition) < 0.3f)
        {
            PickNewTarget();
        }
    }
}
