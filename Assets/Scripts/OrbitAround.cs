using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform orbitCenter;
    public float orbitSpeed = 30f;
    public Vector3 orbitAxis = Vector3.up;

    [Header("Rotation Settings")]
    public float rotateSpeed = 5f;

    void Update()
    {
        if (orbitCenter == null) return;

        transform.RotateAround(
            orbitCenter.position,
            orbitAxis,
            orbitSpeed * Time.deltaTime
        );

        Transform nearest = FindClosestButterfly();

        if (nearest != null)
        {
            Vector3 direction = nearest.position - transform.position;
            direction.y = 0;  

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotateSpeed * Time.deltaTime
                );
            }
        }
    }

    Transform FindClosestButterfly()
    {
        GameObject[] butterflies = GameObject.FindGameObjectsWithTag("Butterfly");

        if (butterflies.Length == 0)
            return null;

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject b in butterflies)
        {
            float dist = Vector3.Distance(transform.position, b.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = b.transform;
            }
        }

        return closest;
    }
}
