using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI lastObjectText;
    public TextMeshProUGUI lastPointsText;
    public TextMeshProUGUI totalScoreText;

    private void Start()
    {
        UpdateUI("-", 0, 0);

        ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
    }

    private void OnDestroy()
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
        lastObjectText.text = $"Last: {name}";
        lastPointsText.text = (delta >= 0) ? $"+{delta}" : $"{delta}";
        totalScoreText.text = $"Total: {total}";
    }
}
