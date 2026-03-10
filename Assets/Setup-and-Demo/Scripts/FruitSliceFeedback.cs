using System.Collections;
using UnityEngine;

public class FruitSliceFeedback : MonoBehaviour
{
    [Header("Target")]
    public Renderer targetRenderer;

    [Header("Flash")]
    [Range(0f, 1f)] public float flashToWhiteAmount = 0.85f;
    public float flashDuration = 0.12f;

    [Header("Emission (optional, if supported by shader)")]
    public bool useEmission = true;
    public float emissionIntensity = 2.5f;

    [Header("Scale Punch")]
    public bool useScalePunch = true;
    public float scaleMultiplier = 1.12f;
    public float scaleDuration = 0.12f;

    MaterialPropertyBlock mpb;
    int baseColorId;
    int colorId;
    int emissionId;

    bool hasBaseColor;
    bool hasColor;
    bool hasEmission;

    Color originalColor = Color.white;
    Color originalEmission = Color.black;

    Vector3 originalScale;
    Coroutine runningRoutine;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogWarning($"[FruitSliceFeedback] No Renderer found on '{name}'.");
            enabled = false;
            return;
        }

        mpb = new MaterialPropertyBlock();

        baseColorId = Shader.PropertyToID("_BaseColor");     // URP/HDRP
        colorId = Shader.PropertyToID("_Color");             // Built-in fallback
        emissionId = Shader.PropertyToID("_EmissionColor");

        var mat = targetRenderer.sharedMaterial;
        if (mat == null)
        {
            Debug.LogWarning($"[FruitSliceFeedback] No material found on '{name}'.");
            enabled = false;
            return;
        }

        hasBaseColor = mat.HasProperty(baseColorId);
        hasColor = mat.HasProperty(colorId);
        hasEmission = mat.HasProperty(emissionId);

        if (hasBaseColor) originalColor = mat.GetColor(baseColorId);
        else if (hasColor) originalColor = mat.GetColor(colorId);

        if (hasEmission) originalEmission = mat.GetColor(emissionId);

        originalScale = transform.localScale;
    }

    public void PlayFeedback()
    {
        if (!enabled) return;

        if (runningRoutine != null)
            StopCoroutine(runningRoutine);

        runningRoutine = StartCoroutine(FeedbackRoutine());
    }

    IEnumerator FeedbackRoutine()
    {
        float t = 0f;
        Vector3 targetScale = originalScale * scaleMultiplier;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / flashDuration);

            // Strong white flash
            Color flashColor = Color.Lerp(originalColor, Color.white, flashToWhiteAmount);

            // Fade from flash back to original over time
            Color currentColor = Color.Lerp(flashColor, originalColor, k);

            targetRenderer.GetPropertyBlock(mpb);

            if (hasBaseColor) mpb.SetColor(baseColorId, currentColor);
            else if (hasColor) mpb.SetColor(colorId, currentColor);

            if (useEmission && hasEmission)
            {
                Color flashEmission = Color.white * emissionIntensity;
                Color currentEmission = Color.Lerp(flashEmission, originalEmission, k);
                mpb.SetColor(emissionId, currentEmission);
            }

            targetRenderer.SetPropertyBlock(mpb);

            if (useScalePunch)
            {
                float scaleK = Mathf.Sin(k * Mathf.PI); // up then down
                transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleK);
            }

            yield return null;
        }

        // Restore original values
        targetRenderer.GetPropertyBlock(mpb);

        if (hasBaseColor) mpb.SetColor(baseColorId, originalColor);
        else if (hasColor) mpb.SetColor(colorId, originalColor);

        if (useEmission && hasEmission)
            mpb.SetColor(emissionId, originalEmission);

        targetRenderer.SetPropertyBlock(mpb);

        transform.localScale = originalScale;
        runningRoutine = null;
    }
}