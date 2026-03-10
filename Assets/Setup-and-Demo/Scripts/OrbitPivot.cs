using UnityEngine;

public class OrbitPivotSpin : MonoBehaviour
{
    public float orbitSpeed = 180f;

    void LateUpdate()
    {
        transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime, Space.Self);
    }
}