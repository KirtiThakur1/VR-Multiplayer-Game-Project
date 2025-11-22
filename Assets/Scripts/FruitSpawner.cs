using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Fruit Prefabs (assign your 10 fruits here)")]
    public GameObject[] fruitPrefabs;   // <- drag all fruit prefabs in Inspector

    [Header("Spawn Area (local XZ) & Height")]
    public Vector2 areaSize = new Vector2(6f, 6f);  // width (X) and depth (Z)
    public float spawnY = 4f;                       // height above spawner

    [Header("Timing")]
    public float spawnInterval = 0.6f;              // time between spawns

    [Header("Spin/Torque")]
    public Vector2 angularSpeed = new Vector2(1f, 3f); // random angular velocity

    [Header("Exclusions (optional - to avoid stairs)")]
    public List<BoxCollider> noSpawnZones = new List<BoxCollider>(); // drag NoSpawn boxes here
    public int maxPlacementTries = 20;

    // Draw the yellow box in Scene view so you see where fruits can appear
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var center = transform.position + new Vector3(0, spawnY, 0);
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, 0.1f, areaSize.y));
    }

    // Coroutine that runs when the game starts
    private IEnumerator Start()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
        {
            Debug.LogError("FruitSpawner: Assign at least one fruit prefab in the inspector!");
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
        // Try several times to find a position outside the noSpawnZones
        for (int i = 0; i < maxPlacementTries; i++)
        {
            // pick a random XZ inside the allowed rectangle
            float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float z = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
            Vector3 pos = transform.position + new Vector3(x, spawnY, z);

            // if this point is inside a forbidden zone (stairs), reject and try again
            if (InsideAnyNoSpawnZone(pos))
                continue;

            // choose a random fruit prefab
            int index = Random.Range(0, fruitPrefabs.Length);
            GameObject prefab = fruitPrefabs[index];

            // instantiate the fruit
            var go = Instantiate(prefab, pos, Random.rotation);

            // give it some random spin
            if (go.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Random.onUnitSphere * Random.Range(angularSpeed.x, angularSpeed.y);
            }

            return; // success, stop the loop
        }

        Debug.LogWarning("FruitSpawner: Could not find spawn point outside noSpawnZones. " +
                         "Increase areaSize or adjust noSpawnZones.");
    }

    private bool InsideAnyNoSpawnZone(Vector3 worldPos)
    {
        if (noSpawnZones == null) return false;

        foreach (var bc in noSpawnZones)
        {
            if (bc == null) continue;
            Bounds b = bc.bounds;

            // ignore Y; check only XZ inside the box
            Vector3 flat = new Vector3(worldPos.x, b.center.y, worldPos.z);
            if (b.Contains(flat))
                return true;
        }

        return false;
    }
}
