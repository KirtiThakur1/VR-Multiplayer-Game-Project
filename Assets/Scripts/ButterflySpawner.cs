using UnityEngine;

public class ButterflySpawner : MonoBehaviour
{
    public GameObject butterflyPrefab;
    public int count = 5;
    public Vector3 areaSize = new Vector3(5f, 2f, 5f);

    void Start()
    {
        if (butterflyPrefab == null)
        {
            Debug.LogWarning("ButterflySpawner: butterflyPrefab is not assigned!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                0f,
                Random.Range(-areaSize.z / 2f, areaSize.z / 2f)
            );

            GameObject b = Instantiate(butterflyPrefab, randomPos, Quaternion.identity);

            var move = b.GetComponent<ButterflyMovement>();
            if (move != null)
            {
                move.areaCenter = transform.position; 
                move.areaSize = areaSize;            
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
