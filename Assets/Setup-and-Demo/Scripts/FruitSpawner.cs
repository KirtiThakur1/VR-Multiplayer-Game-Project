using System.Collections;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject fruitPrefab;

    [Header("Spawn Area (XZ) & Height")]
    public Vector2 areaSize = new Vector2(6f, 6f); // width x depth
    public float spawnY = 4f;                       // how high above the spawner

    [Header("Timing")]
    public float spawnInterval = 0.6f;

    [Header("Spin/Torque")]
    public Vector2 angularSpeed = new Vector2(1f, 3f); // random angular velocity

    private void OnDrawGizmosSelected()
    {
        // visualize the spawn rectangle at height spawnY
        Gizmos.color = Color.yellow;
        var center = transform.position + new Vector3(0, spawnY, 0);
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, 0.1f, areaSize.y));
    }

    private IEnumerator Start()
    {
        if (fruitPrefab == null)
        {
            Debug.LogError("FruitSpawner: Assign 'fruitPrefab' in the inspector.");
            yield break;
        }

        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float z = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);

        Vector3 pos = transform.position + new Vector3(x, spawnY, z);
        Quaternion rot = Random.rotation;

        var go = Instantiate(fruitPrefab, pos, rot);

        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            // let gravity do the falling; add a little random spin
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Random.onUnitSphere * Random.Range(angularSpeed.x, angularSpeed.y);
        }
    }
}
