using UnityEngine;

public class AxeSlicer : MonoBehaviour
{
    [Header("Slice Settings")]
    public float minSliceSpeed = 1f; 

    private Vector3 lastPosition;
    private Vector3 velocity;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Fruit"))
            return;

        if (velocity.magnitude < minSliceSpeed)
            return;

        FruitHitEffect hit = other.GetComponent<FruitHitEffect>();

        if (hit != null)
        {
            hit.OnSliced();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
