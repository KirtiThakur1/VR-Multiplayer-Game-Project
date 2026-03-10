using TMPro;
using UnityEngine;

public class FloatingScoreText : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro textMesh;

    [Header("Motion")]
    public float moveSpeed = 0.5f;
    public float lifetime = 1.0f;

    [Header("Fade")]
    public bool fadeOut = true;

    [Header("Billboard To HMD")]
    public bool followHeadRotation = true;

    private Color startColor;
    private float timer;
    private Transform headTransform;

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        if (textMesh != null)
            startColor = textMesh.color;

        FindHeadTransform();
    }

    public void Setup(string message, Color color)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        textMesh.text = message;
        textMesh.color = color;
        startColor = color;

        if (headTransform == null)
            FindHeadTransform();
    }

    void LateUpdate()
    {
        timer += Time.deltaTime;

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (followHeadRotation)
        {
            if (headTransform == null)
                FindHeadTransform();

            if (headTransform != null)
            {
                transform.rotation = headTransform.rotation * Quaternion.Euler(0f, 180f, 0f);
            }
        }

        if (fadeOut && textMesh != null)
        {
            float t = timer / lifetime;
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            textMesh.color = c;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void FindHeadTransform()
    {
        if (Camera.main != null)
        {
            headTransform = Camera.main.transform;
        }
    }
}