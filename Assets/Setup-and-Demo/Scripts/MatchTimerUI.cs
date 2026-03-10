using UnityEngine;
using TMPro;

public class MatchTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private void Start()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnTimerChanged += HandleTimerChanged;
            HandleTimerChanged(MatchManager.Instance.TimeLeft);
        }
    }

    private void OnDestroy()
    {
        if (MatchManager.Instance != null)
            MatchManager.Instance.OnTimerChanged -= HandleTimerChanged;
    }

    private void HandleTimerChanged(float timeLeft)
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        if (seconds < 0) seconds = 0;

        if (timerText != null)
            timerText.text = $"Time: {seconds}";
    }
}