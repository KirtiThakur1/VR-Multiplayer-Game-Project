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

    [Header("When Released: return to orbit")]
    public float returnToOrbitSpeed = 6f; // higher = faster snap back

    [Header("Orientation (NOT butterflies)")]
    public bool faceTangentDirection = true;
    public float rotateSpeed = 8f;

    XRGrabInteractable _grab;
    bool _isHeld;

    // orbit state
    float _angleDeg;               // current orbit angle
    Vector3 _axisNorm;
    Vector3 _refDir;               // reference direction from center
    Vector3 _lastOrbitPos;

    void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        if (_grab != null)
        {
            _grab.selectEntered.AddListener(_ => _isHeld = true);
            _grab.selectExited.AddListener(_ => OnReleased());
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

        _axisNorm = orbitAxis.sqrMagnitude > 0.0001f ? orbitAxis.normalized : Vector3.up;

        // Initialize orbit angle and radius based on current position in scene
        Vector3 fromCenter = transform.position - orbitCenter.position;

        // Project onto orbit plane (remove axis component)
        fromCenter = Vector3.ProjectOnPlane(fromCenter, _axisNorm);

        if (fromCenter.sqrMagnitude < 0.0001f)
        {
            // fallback if placed exactly on center
            fromCenter = Vector3.right * Mathf.Max(orbitRadius, 0.01f);
        }

        orbitRadius = fromCenter.magnitude;

        _refDir = fromCenter.normalized; // initial reference direction

        // Set initial angle = 0 at refDir
        _angleDeg = 0f;

        _lastOrbitPos = orbitCenter.position + _refDir * orbitRadius;
    }

    void OnReleased()
    {
        _isHeld = false;

        // When released, recompute orbit phase from current position so it returns smoothly
        Vector3 fromCenter = transform.position - orbitCenter.position;
        fromCenter = Vector3.ProjectOnPlane(fromCenter, _axisNorm);

        if (fromCenter.sqrMagnitude > 0.0001f)
        {
            // Update radius to current (optional). If you want fixed radius always, comment the next line.
            orbitRadius = fromCenter.magnitude;

            // Compute signed angle between refDir and current dir around axis
            Vector3 curDir = fromCenter.normalized;
            _angleDeg = SignedAngleOnAxis(_refDir, curDir, _axisNorm);
        }
    }

    void Update()
    {
        if (orbitCenter == null) return;

        if (_isHeld)
            return; // hand controls it while held

        // Advance orbit
        _angleDeg += orbitSpeedDeg * Time.deltaTime;

        // Desired orbit position
        Quaternion q = Quaternion.AngleAxis(_angleDeg, _axisNorm);
        Vector3 desiredPos = orbitCenter.position + (q * _refDir) * orbitRadius;

        // Smooth return / follow orbit
        float k = 1f - Mathf.Exp(-returnToOrbitSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, k);

        // Optional: face tangent direction (makes orbit readable, independent of butterflies)
        if (faceTangentDirection)
        {
            Vector3 tangent = (desiredPos - _lastOrbitPos);
            _lastOrbitPos = desiredPos;

            if (tangent.sqrMagnitude > 0.000001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(tangent.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }
    }

    static float SignedAngleOnAxis(Vector3 from, Vector3 to, Vector3 axis)
    {
        // signed angle around axis
        Vector3 f = Vector3.ProjectOnPlane(from, axis).normalized;
        Vector3 t = Vector3.ProjectOnPlane(to, axis).normalized;

        float angle = Vector3.SignedAngle(f, t, axis);
        return angle;
    }

    void OnDestroy()
    {
        if (_grab != null)
        {
            _grab.selectEntered.RemoveAllListeners();
            _grab.selectExited.RemoveAllListeners();
        }
    }
}