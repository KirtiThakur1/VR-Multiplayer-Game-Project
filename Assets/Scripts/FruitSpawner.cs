// Scripts/FruitSpawner.cs
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public GameObject[] fruitPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 1.2f;
    public Vector2 upForce = new Vector2(6f, 10f);
    public Vector2 sideForce = new Vector2(-2f, 2f);

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnFruit();
        }
    }

    void SpawnFruit()
    {
        if (fruitPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        var prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
        var sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        var go = Instantiate(prefab, sp.position, Random.rotation);
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float up = Random.Range(upForce.x, upForce.y);
            float side = Random.Range(sideForce.x, sideForce.y);
            Vector3 impulse = Vector3.up * up + sp.right * side;
            rb.AddForce(impulse, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
    }
}
