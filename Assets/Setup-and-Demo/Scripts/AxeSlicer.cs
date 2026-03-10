using UnityEngine;

public class AxeSlicer : MonoBehaviour
{
    [Header("Owner")]
    public PlayerScore ownerScore;

    [Header("Slice Settings")]
    public float minSliceSpeed = 0.8f;
    public float velocitySmoothing = 20f;
    public float sliceCooldown = 0.05f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip fruitSliceSound;
    public AudioClip bombSliceSound;
    [Range(0f, 1f)] public float sliceVolume = 1f;

    [Header("Floating Score Text")]
    public GameObject floatingTextPrefab;
    public Vector3 floatingTextOffset = new Vector3(0f, 0.5f, 0f);
    public Color fruitTextColor = Color.green;
    public Color bombTextColor = Color.red;

    private Vector3 lastPosition;
    private Vector3 smoothedVelocity;
    private float lastSliceTime;

    void Start()
    {
        lastPosition = transform.position;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 instantVelocity = (transform.position - lastPosition) / dt;
        lastPosition = transform.position;

        float k = 1f - Mathf.Exp(-velocitySmoothing * dt);
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, instantVelocity, k);
    }

    void OnTriggerEnter(Collider other)
    {
        TrySlice(other);
    }

    void OnTriggerStay(Collider other)
    {
        TrySlice(other);
    }

    void TrySlice(Collider other)
    {
        if (ownerScore == null)
            return;

        if (Time.time - lastSliceTime < sliceCooldown)
            return;

        float speed = smoothedVelocity.magnitude;
        if (speed < minSliceSpeed)
            return;

        // -----------------------------
        // 1) Bomb check
        // -----------------------------
        Transform root = other.transform.root;
        if (root.CompareTag("Bomb"))
        {
            lastSliceTime = Time.time;

            PlaySound(bombSliceSound);

            ownerScore.RegisterBombSlice();

            SpawnFloatingText(root.position, "-20", bombTextColor);

            Destroy(root.gameObject);
            return;
        }

        // -----------------------------
        // 2) Fruit check
        // -----------------------------
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit == null)
            fruit = other.GetComponentInParent<Fruit>();

        if (fruit == null)
            return;

        if (fruit.sliced)
            return;

        lastSliceTime = Time.time;

        fruit.sliced = true;

        PlaySound(fruitSliceSound);

        ownerScore.RegisterFruitSlice(fruit);

        SpawnFloatingText(fruit.transform.position, $"+{fruit.baseScore}", fruitTextColor);

        FruitHitEffect hit = other.GetComponent<FruitHitEffect>();
        if (hit == null)
            hit = other.GetComponentInParent<FruitHitEffect>();

        if (hit != null)
            hit.OnSliced();
        else
            fruit.Slice();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, sliceVolume);
        }
    }

    void SpawnFloatingText(Vector3 worldPosition, string message, Color color)
    {
        if (floatingTextPrefab == null)
            return;

        GameObject obj = Instantiate(
            floatingTextPrefab,
            worldPosition + floatingTextOffset,
            Quaternion.identity
        );

        FloatingScoreText floating = obj.GetComponent<FloatingScoreText>();
        if (floating != null)
        {
            floating.Setup(message, color);
        }
    }
}