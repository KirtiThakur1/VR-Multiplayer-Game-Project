using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI lastObjectText;
    public TextMeshProUGUI lastPointsText;
    public TextMeshProUGUI totalScoreText;

    private void OnEnable()
    {
        UpdateUI("-", 0, ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(string name, int delta, int total)
    {
        UpdateUI(name, delta, total);
    }

    private void UpdateUI(string name, int delta, int total)
    {
        if (lastObjectText != null) lastObjectText.text = $"Last: {name}";
        if (lastPointsText != null) lastPointsText.text = (delta >= 0) ? $"+{delta}" : $"{delta}";
        if (totalScoreText != null) totalScoreText.text = $"Total: {total}";
    }
}

