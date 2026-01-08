using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI lastObjectText;
    public TextMeshProUGUI lastPointsText;
    public TextMeshProUGUI totalScoreText;

    private bool subscribed = false;

    private void Start()
    {
        TrySubscribe();
        UpdateUI("-", 0, ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0);
    }

    private void OnEnable()
    {
        TrySubscribe();
        UpdateUI("-", 0, ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0);
    }

    private void OnDisable()
    {
        TryUnsubscribe();
    }

    private void OnDestroy()
    {
        TryUnsubscribe();
    }

    private void TrySubscribe()
    {
        if (!subscribed && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            subscribed = true;
            Debug.Log("ScoreUI subscribed to ScoreManager.OnScoreChanged");
        }
    }

    private void TryUnsubscribe()
    {
        if (subscribed && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            subscribed = false;
            Debug.Log("ScoreUI unsubscribed");
        }
    }

    private void HandleScoreChanged(string name, int delta, int total)
    {
        name = name.Replace("(Clone)", "").Trim();

        UpdateUI(name, delta, total);
    }

    private void UpdateUI(string name, int delta, int total)
    {
        if (lastObjectText != null) lastObjectText.text = $"Last: {name}";
        if (lastPointsText != null) lastPointsText.text = (delta >= 0) ? $"+{delta}" : $"{delta}";
        if (totalScoreText != null) totalScoreText.text = $"Total: {total}";
    }
}
