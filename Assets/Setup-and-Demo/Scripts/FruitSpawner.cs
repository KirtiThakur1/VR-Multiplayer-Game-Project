using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Fruit Prefabs (assign your 10 fruits here)")]
    public GameObject[] fruitPrefabs;

    [Header("Spawn Area (local XZ) & Height")]
    public Vector2 areaSize = new Vector2(6f, 6f);
    public float spawnY = 4f;

    [Header("Timing")]
    public float spawnInterval = 0.6f;

    [Header("Spin/Torque")]
    public Vector2 angularSpeed = new Vector2(1f, 3f);

    [Header("Fall Control (make fruits fall slowly)")]
    [Tooltip("Initial downward speed applied at spawn (smaller = slower).")]
    public float initialDownSpeed = 0.4f;

    [Tooltip("Hard cap for falling speed (prevents fruits from becoming too fast).")]
    public float maxFallSpeed = 1.8f;

    [Tooltip("Adds air resistance. Higher = slower and smoother motion.")]
    public float linearDamping = 2.5f;

    [Header("Exclusions (optional - to avoid stairs)")]
    public List<BoxCollider> noSpawnZones = new List<BoxCollider>();
    public int maxPlacementTries = 20;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnMatchEnded += HandleMatchEnded;
        }
    }

    private void OnDisable()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnMatchEnded -= HandleMatchEnded;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var center = transform.position + new Vector3(0, spawnY, 0);
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, 0.1f, areaSize.y));
    }

    private IEnumerator Start()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
        {
            Debug.LogError("FruitSpawner: Assign at least one fruit prefab in the inspector!");
            yield break;
        }

        yield return new WaitUntil(() => MatchManager.Instance != null);

        yield return new WaitUntil(() => MatchManager.Instance.IsMatchRunning);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (enabled && MatchManager.Instance != null && MatchManager.Instance.IsMatchRunning)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }

        spawnRoutine = null;
    }

    private void HandleMatchEnded(PlayerScore winner, PlayerScore loser)
    {
        StopSpawning();
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        enabled = false;
    }

    private void SpawnOne()
    {
        if (MatchManager.Instance != null && !MatchManager.Instance.IsMatchRunning)
            return;

        for (int i = 0; i < maxPlacementTries; i++)
        {
            float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float z = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
            Vector3 pos = transform.position + new Vector3(x, spawnY, z);

            if (InsideAnyNoSpawnZone(pos))
                continue;

            int index = Random.Range(0, fruitPrefabs.Length);
            GameObject prefab = fruitPrefabs[index];

            var go = Instantiate(prefab, pos, Random.rotation);

            if (go.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = new Vector3(0f, -Mathf.Abs(initialDownSpeed), 0f);
                rb.linearDamping = Mathf.Max(0f, linearDamping);
                rb.angularVelocity = Random.onUnitSphere * Random.Range(angularSpeed.x, angularSpeed.y);

                if (maxFallSpeed > 0f)
                {
                    var limiter = go.GetComponent<LimitFallSpeed>();
                    if (limiter == null) limiter = go.AddComponent<LimitFallSpeed>();
                    limiter.maxFallSpeed = maxFallSpeed;
                }
            }

            return;
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

            Vector3 flat = new Vector3(worldPos.x, b.center.y, worldPos.z);
            if (b.Contains(flat))
                return true;
        }

        return false;
    }
}