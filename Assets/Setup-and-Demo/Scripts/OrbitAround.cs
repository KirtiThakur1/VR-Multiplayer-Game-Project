using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrbitAround : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform orbitCenter;
    public Vector3 orbitAxis = Vector3.up;
    public float orbitSpeedDeg = 60f;
    public float orbitRadius = 0.7f;

    [Header("Return To Grab Point")]
    public float returnMoveSpeed = 2.5f;
    public float returnRotateSpeed = 10f;
    public float snapDistance = 0.02f;

    [Header("Orientation")]
    public bool faceTangentDirection = true;
    public float orbitRotateSpeed = 8f;

    XRGrabInteractable grab;
    Rigidbody rb;

    enum OrbitState
    {
        Orbiting,
        Held,
        Returning
    }

    OrbitState state = OrbitState.Orbiting;

    Vector3 axisNorm;
    Vector3 refDir;

    float angleDeg;
    float savedGrabAngleDeg;

    Vector3 lastOrbitPos;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }
    }

    void Start()
    {
        if (orbitCenter == null)
        {
            Debug.LogError("[OrbitAround] orbitCenter is not assigned.");
            enabled = false;
            return;
        }

        axisNorm = orbitAxis.sqrMagnitude > 0.0001f ? orbitAxis.normalized : Vector3.up;

        Vector3 fromCenter = transform.position - orbitCenter.position;
        fromCenter = Vector3.ProjectOnPlane(fromCenter, axisNorm);

        if (fromCenter.sqrMagnitude < 0.0001f)
            fromCenter = Vector3.right * Mathf.Max(orbitRadius, 0.01f);

        orbitRadius = fromCenter.magnitude;
        refDir = fromCenter.normalized;

        angleDeg = 0f;
        savedGrabAngleDeg = 0f;

        lastOrbitPos = transform.position;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        state = OrbitState.Held;

        // دقیقا زاویه فعلی روی مدار ذخیره می‌شود
        savedGrabAngleDeg = angleDeg;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        state = OrbitState.Returning;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (orbitCenter == null) return;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        switch (state)
        {
            case OrbitState.Held:
                // وقتی دست کاربر گرفته، هیچ orbitی انجام نمی‌ده
                return;

            case OrbitState.Returning:
                ReturnToSavedGrabPoint();
                return;

            case OrbitState.Orbiting:
                OrbitNormally();
                return;
        }
    }

    void OrbitNormally()
    {
        angleDeg += orbitSpeedDeg * Time.deltaTime;

        Quaternion q = Quaternion.AngleAxis(angleDeg, axisNorm);
        Vector3 desiredPos = orbitCenter.position + (q * refDir) * orbitRadius;

        transform.position = desiredPos;

        if (faceTangentDirection)
        {
            Vector3 tangent = (desiredPos - lastOrbitPos);
            if (tangent.sqrMagnitude > 0.000001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(tangent.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    orbitRotateSpeed * Time.deltaTime
                );
            }
        }

        lastOrbitPos = desiredPos;
    }

    void ReturnToSavedGrabPoint()
    {
        Quaternion q = Quaternion.AngleAxis(savedGrabAngleDeg, axisNorm);
        Vector3 targetPos = orbitCenter.position + (q * refDir) * orbitRadius;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            returnMoveSpeed * Time.deltaTime
        );

        if (faceTangentDirection)
        {
            Vector3 tangentDir = Vector3.Cross(axisNorm, (targetPos - orbitCenter.position).normalized);
            if (tangentDir.sqrMagnitude > 0.000001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(tangentDir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    returnRotateSpeed * Time.deltaTime
                );
            }
        }

        if (Vector3.Distance(transform.position, targetPos) <= snapDistance)
        {
            transform.position = targetPos;
            angleDeg = savedGrabAngleDeg;
            lastOrbitPos = targetPos;
            state = OrbitState.Orbiting;
        }
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }
}