using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreboardUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    private readonly List<PlayerScore> subscribedPlayers = new List<PlayerScore>();

    private void Update()
    {
        EnsurePlayerSubscriptions();
        RefreshScores();
    }

    private void Start()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnTimerChanged += HandleTimerChanged;
            MatchManager.Instance.OnMatchEnded += HandleMatchEnded;
            HandleTimerChanged(MatchManager.Instance.TimeLeft);
        }

        RefreshScores();
    }

    private void OnDestroy()
    {
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnTimerChanged -= HandleTimerChanged;
            MatchManager.Instance.OnMatchEnded -= HandleMatchEnded;
        }

        foreach (var player in subscribedPlayers)
        {
            if (player != null)
                player.OnScoreChanged -= HandlePlayerScoreChanged;
        }

        subscribedPlayers.Clear();
    }

    private void EnsurePlayerSubscriptions()
    {
        if (MatchManager.Instance == null) return;

        foreach (var player in MatchManager.Instance.players)
        {
            if (player != null && !subscribedPlayers.Contains(player))
            {
                player.OnScoreChanged += HandlePlayerScoreChanged;
                subscribedPlayers.Add(player);
            }
        }
    }

    private void HandlePlayerScoreChanged(PlayerScore player)
    {
        RefreshScores();
    }

    private void HandleTimerChanged(float timeLeft)
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        if (seconds < 0) seconds = 0;

        if (timerText != null)
            timerText.text = $"Time: {seconds}";
    }

    private void HandleMatchEnded(PlayerScore winner, PlayerScore loser)
    {
        RefreshScores();
    }

    private void RefreshScores()
    {
        if (MatchManager.Instance == null) return;

        var players = MatchManager.Instance.players;

        int score1 = players.Count > 0 && players[0] != null ? players[0].CurrentScore : 0;
        int score2 = players.Count > 1 && players[1] != null ? players[1].CurrentScore : 0;

        if (player1ScoreText != null)
            player1ScoreText.text = $"Player 1: {score1}";

        if (player2ScoreText != null)
            player2ScoreText.text = $"Player 2: {score2}";
    }
}