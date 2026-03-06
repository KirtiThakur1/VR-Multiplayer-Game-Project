using UnityEngine;

public class FruitPulseByTime : MonoBehaviour
{
    [Header("Target Renderer (optional)")]
    public Renderer targetRenderer;

    [Header("Pulse Settings")]
    public float frequency = 2f;
    public bool randomizePhase = true;

    [Tooltip("How dark it gets at the low point (0.2 = much darker).")]
    [Range(0.05f, 1f)] public float darkFactor = 0.2f;

    [Tooltip("How bright it gets at the high point (1.0 keeps original).")]
    [Range(1f, 2.5f)] public float brightFactor = 1.6f;

    MaterialPropertyBlock mpb;
    int baseColorId;
    int colorId;

    bool hasBaseColor;
    bool hasColor;

    Color baseColor = Color.white;
    float phase;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogWarning($"[FruitPulseByTime] No Renderer found on '{name}'.");
            enabled = false;
            return;
        }

        mpb = new MaterialPropertyBlock();
        baseColorId = Shader.PropertyToID("_BaseColor"); // URP/Lit
        colorId = Shader.PropertyToID("_Color");         // Built-in fallback

        var mat = targetRenderer.sharedMaterial;
        if (mat == null)
        {
            Debug.LogWarning($"[FruitPulseByTime] Renderer on '{name}' has no material.");
            enabled = false;
            return;
        }

        hasBaseColor = mat.HasProperty(baseColorId);
        hasColor = mat.HasProperty(colorId);

        if (hasBaseColor) baseColor = mat.GetColor(baseColorId);
        else if (hasColor) baseColor = mat.GetColor(colorId);
        else
        {
            Debug.LogWarning($"[FruitPulseByTime] '{name}' material has neither _BaseColor nor _Color.");
            enabled = false;
            return;
        }

        phase = randomizePhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    void Update()
    {
        float w = Mathf.PI * 2f * frequency;
        float s = (Mathf.Sin(Time.time * w + phase) + 1f) * 0.5f; // 0..1

        Color low = baseColor * darkFactor;
        Color high = baseColor * brightFactor;
        Color pulsed = Color.Lerp(low, high, s);

        targetRenderer.GetPropertyBlock(mpb);

        if (hasBaseColor) mpb.SetColor(baseColorId, pulsed);
        else mpb.SetColor(colorId, pulsed);

        targetRenderer.SetPropertyBlock(mpb);
    }
}