using UnityEngine;
using TMPro;

public class PlayerEndGameUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI winnerText;

    [Header("Bound Player")]
    public PlayerScore targetPlayerScore;

    private void Start()
    {
        if (root != null)
            root.SetActive(false);

        if (MatchManager.Instance != null)
            MatchManager.Instance.OnMatchEnded += HandleMatchEnded;
    }

    private void OnDestroy()
    {
        if (MatchManager.Instance != null)
            MatchManager.Instance.OnMatchEnded -= HandleMatchEnded;
    }

    public void Bind(PlayerScore playerScore)
    {
        targetPlayerScore = playerScore;
    }

    private void HandleMatchEnded(PlayerScore winner, PlayerScore loser)
    {
        if (root == null || targetPlayerScore == null)
            return;

        root.SetActive(true);

        if (winner == null)
        {
            if (titleText != null)
                titleText.text = "DRAW";

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {targetPlayerScore.CurrentScore}";

            if (winnerText != null)
                winnerText.text = "No winner";
            return;
        }

        bool isWinner = winner == targetPlayerScore;

        if (titleText != null)
        {
            if (isWinner)
                titleText.text = "YOU WON";
            else
                titleText.text = "GAME OVER";
        }

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {targetPlayerScore.CurrentScore}";

        if (winnerText != null)
            winnerText.text = $"Winner: {winner.displayName}";
    }
}