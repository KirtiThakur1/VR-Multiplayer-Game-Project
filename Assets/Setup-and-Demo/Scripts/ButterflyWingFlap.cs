using UnityEngine;

public class ButterflyWingFlap : MonoBehaviour
{
    [Header("Wing References")]
    public Transform leftWing;
    public Transform rightWing;

    [Header("Flap Settings")]
    public float flapSpeed = 10f;
    public float flapAngle = 30f;

    [Header("Rotation Axis")]
    public Vector3 leftAxis = new Vector3(1, 0, 0);
    public Vector3 rightAxis = new Vector3(1, 0, 0);

    private Quaternion leftBaseRot;
    private Quaternion rightBaseRot;

    void Start()
    {
        if (leftWing != null)
            leftBaseRot = leftWing.localRotation;

        if (rightWing != null)
            rightBaseRot = rightWing.localRotation;
    }

    void Update()
    {
        if (leftWing == null || rightWing == null)
            return;

        float angle = Mathf.Sin(Time.time * flapSpeed) * flapAngle;

        leftWing.localRotation = leftBaseRot * Quaternion.AngleAxis(angle, leftAxis);
        rightWing.localRotation = rightBaseRot * Quaternion.AngleAxis(-angle, rightAxis);
    }
}

