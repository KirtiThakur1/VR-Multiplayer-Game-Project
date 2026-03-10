using UnityEngine;
using TMPro;

public class PlayerScoreUI : MonoBehaviour
{
    public PlayerScore targetScore;

    public TextMeshProUGUI lastObjectText;
    public TextMeshProUGUI lastPointsText;
    public TextMeshProUGUI totalScoreText;

    private void OnEnable()
    {
        if (targetScore != null)
        {
            targetScore.OnScoreChanged += HandleScoreChanged;
            Refresh();
        }
    }

    private void OnDisable()
    {
        if (targetScore != null)
            targetScore.OnScoreChanged -= HandleScoreChanged;
    }

    public void Bind(PlayerScore score)
    {
        if (targetScore != null)
            targetScore.OnScoreChanged -= HandleScoreChanged;

        targetScore = score;

        if (targetScore != null)
            targetScore.OnScoreChanged += HandleScoreChanged;

        Refresh();
    }

    private void HandleScoreChanged(PlayerScore score)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (targetScore == null)
        {
            if (lastObjectText != null) lastObjectText.text = "Last: -";
            if (lastPointsText != null) lastPointsText.text = "0";
            if (totalScoreText != null) totalScoreText.text = "Total: 0";
            return;
        }

        if (lastObjectText != null)
            lastObjectText.text = $"Last: {targetScore.LastObjectName}";

        if (lastPointsText != null)
            lastPointsText.text = targetScore.LastDelta >= 0
                ? $"+{targetScore.LastDelta}"
                : $"{targetScore.LastDelta}";

        if (totalScoreText != null)
            totalScoreText.text = $"Total: {targetScore.CurrentScore}";
    }
}