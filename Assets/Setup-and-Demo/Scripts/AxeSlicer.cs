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
        float dt = Time.deltaTime;
        if (dt > 0f)
            velocity = (transform.position - lastPosition) / dt;

        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Axe hit: " + other.name);

        if (!other.CompareTag("Fruit"))
            return;


        if (velocity.magnitude < minSliceSpeed)
            return;

        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit == null)
            fruit = other.GetComponentInParent<Fruit>();

        if (fruit == null || fruit.sliced)
            return;

        // 1️⃣ Mark sliced FIRST
        fruit.sliced = true;

        // 2️⃣ Register score FIRST
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterFruitSlice(fruit);

        // 3️⃣ Visual / FX
        FruitHitEffect hit = other.GetComponent<FruitHitEffect>();
        if (hit != null)
            hit.OnSliced();
        else
            fruit.Slice();
    }
}
