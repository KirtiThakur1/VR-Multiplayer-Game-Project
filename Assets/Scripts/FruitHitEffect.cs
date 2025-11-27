using System.Collections;
using UnityEngine;

public class FruitHitEffect : MonoBehaviour
{
    public Renderer rend;
    public Color hitColor = Color.white;
    public float flashDuration = 0.2f;

    private Color originalColor;
    private Coroutine flashRoutine;

    void Start()
    {
        if (rend == null)
        {
            rend = GetComponentInChildren<Renderer>();
        }

        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    public void OnSliced()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashAndDie());
    }

    private IEnumerator FlashAndDie()
    {
        float t = 0f;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float lerp = t / flashDuration;
            if (rend != null)
            {
                rend.material.color = Color.Lerp(originalColor, hitColor, lerp);
            }
            yield return null;
        }

        Destroy(gameObject); // بعد از افکت، میوه حذف میشه
    }
}
