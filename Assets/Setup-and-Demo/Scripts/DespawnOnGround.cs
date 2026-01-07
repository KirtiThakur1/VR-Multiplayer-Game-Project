using UnityEngine;

public class DespawnOnGround : MonoBehaviour
{
    [Tooltip("The tag used by ground objects")]
    public string groundTag = "Ground";

    public float destroyDelay = 0f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.CompareTag(groundTag))
        {
            Debug.Log($"[Despawn] Collision with {collision.collider.name} (tag={collision.collider.tag})");
            Destroy(gameObject, destroyDelay);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag(groundTag))
        {
            Debug.Log($"[Despawn] Trigger with {other.name} (tag={other.tag})");
            Destroy(gameObject, destroyDelay);
        }
    }
}
