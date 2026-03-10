using System.Collections;
using UnityEngine;

public class FruitHitEffect : MonoBehaviour
{
    [Header("Visual Feedback")]
    public Renderer rend;
    public Color hitColor = Color.white;
    public float flashDuration = 0.15f;

    [Header("Scale Punch")]
    public bool useScalePunch = true;
    public float scaleMultiplier = 1.15f;

    private Color originalColor;
    private Vector3 originalScale;
    private Coroutine flashRoutine;
    private bool sliced;

    private FruitPulseByTime pulseEffect;

    void Start()
    {
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            originalColor = rend.material.color;

        originalScale = transform.localScale;

        pulseEffect = GetComponent<FruitPulseByTime>();
        if (pulseEffect == null)
            pulseEffect = GetComponentInChildren<FruitPulseByTime>();
    }

    public void OnSliced()
    {
        if (sliced) return;
        sliced = true;

        if (pulseEffect != null)
            pulseEffect.StopPulseAndClear();

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashAndDie());
    }

    private IEnumerator FlashAndDie()
    {
        float t = 0f;
        Vector3 targetScale = originalScale * scaleMultiplier;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / flashDuration);

            if (rend != null)
            {
                Color currentColor = Color.Lerp(hitColor, originalColor, k);
                rend.material.color = currentColor;
            }

            if (useScalePunch)
            {
                float scaleK = Mathf.Sin(k * Mathf.PI);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleK);
            }

            yield return null;
        }

        if (rend != null)
            rend.material.color = originalColor;

        transform.localScale = originalScale;

        Destroy(gameObject);
    }
}