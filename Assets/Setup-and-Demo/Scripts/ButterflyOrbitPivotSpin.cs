using UnityEngine;

public class ButterflyOrbitPivotSpin : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speedDeg = 180f;

    void Update()
    {
        transform.Rotate(axis, speedDeg * Time.deltaTime, Space.Self);
    }
}