using UnityEngine;
using TMPro;

public class MatchResultUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject root;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI finalScoreText;

    private void Start()
    {
        if (root != null)
            root.SetActive(false);

        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnMatchEnded += HandleMatchEnded;
        }
    }

    private void OnDestroy()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnMatchEnded -= HandleMatchEnded;
        }
    }

    private void HandleMatchEnded(PlayerScore winner, PlayerScore loser)
    {
        Debug.Log("MatchResultUI: Match ended");

        if (root == null)
        {
            Debug.LogWarning("Result UI root is NULL");
            return;
        }

        root.SetActive(true);

        if (winner == null)
        {
            if (resultText != null)
                resultText.text = "DRAW";

            if (finalScoreText != null && MatchManager.Instance.players.Count >= 2)
            {
                int p1 = MatchManager.Instance.players[0].CurrentScore;
                int p2 = MatchManager.Instance.players[1].CurrentScore;

                finalScoreText.text = $"Player1: {p1}  |  Player2: {p2}";
            }
        }
        else
        {
            if (resultText != null)
                resultText.text = $"WINNER: {winner.displayName}";

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {winner.CurrentScore}";
        }
    }
}