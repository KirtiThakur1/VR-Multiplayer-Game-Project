using UnityEngine;

public class FaceVelocitySmooth : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("How fast the object rotates toward its movement direction.")]
    public float rotateSpeed = 12f;

    [Tooltip("If true, keeps the object upright (rotates only around Y). Recommended for VR readability.")]
    public bool lockY = true;

    [Header("Debug (Scene View only)")]
    [Tooltip("Draw debug rays in the Scene view while playing.")]
    public bool drawDebugRays = true;

    [Tooltip("Length of the debug rays.")]
    public float debugRayLength = 0.3f;

    [Header("Optional: Smooth the velocity to reduce jitter")]
    [Tooltip("Higher = more responsive, lower = smoother (less jitter). 0 disables smoothing.")]
    public float velocitySmoothing = 20f;

    private Vector3 lastPos;
    private Vector3 smoothedVel;

    void Start()
    {
        lastPos = transform.position;
        smoothedVel = Vector3.zero;
    }

    void LateUpdate()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);

        // Instant velocity
        Vector3 instantVel = (transform.position - lastPos) / dt;
        lastPos = transform.position;

        // Optional exponential smoothing (good for VR jitter)
        Vector3 velToUse = instantVel;
        if (velocitySmoothing > 0f)
        {
            float k = 1f - Mathf.Exp(-velocitySmoothing * dt);
            smoothedVel = Vector3.Lerp(smoothedVel, instantVel, k);
            velToUse = smoothedVel;
        }

        if (velToUse.sqrMagnitude < 0.00001f)
            return;

        if (lockY)
            velToUse.y = 0f;

        if (velToUse.sqrMagnitude < 0.00001f)
            return;

        // Target rotation: face the movement direction
        Quaternion targetRot = Quaternion.LookRotation(velToUse.normalized, Vector3.up);

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * dt);

        // Debug rays:
        // Green = velocity direction (what we WANT to face)
        // Red   = current forward direction (what we ARE facing)
        if (drawDebugRays)
        {
            Debug.DrawRay(transform.position, velToUse.normalized * debugRayLength, Color.green);
            Debug.DrawRay(transform.position, transform.forward * debugRayLength, Color.red);
        }
    }
}