using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    [Header("Match Settings")]
    public float matchDuration = 60f;
    public bool startWithOnePlayerForTesting = true;

    [Header("Registered Players")]
    public List<PlayerScore> players = new List<PlayerScore>();

    public bool IsMatchRunning { get; private set; }
    public float TimeLeft { get; private set; }

    public Action<float> OnTimerChanged;
    public Action<PlayerScore, PlayerScore> OnMatchEnded;

    private bool matchStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterPlayer(PlayerScore player)
    {
        if (player == null) return;
        if (players.Contains(player)) return;

        players.Add(player);
        Debug.Log($"Registered player: {player.name}");

        if (!matchStarted)
        {
            if (startWithOnePlayerForTesting && players.Count >= 1)
            {
                StartMatch();
            }
            else if (!startWithOnePlayerForTesting && players.Count >= 2)
            {
                StartMatch();
            }
        }
    }

    public void StartMatch()
    {
        StopAllCoroutines();

        foreach (var player in players)
        {
            if (player != null)
                player.ResetScore();
        }

        StartCoroutine(MatchRoutine());
    }

    private IEnumerator MatchRoutine()
    {
        matchStarted = true;
        IsMatchRunning = true;

        TimeLeft = matchDuration;
        OnTimerChanged?.Invoke(TimeLeft);

        while (TimeLeft > 0f)
        {
            TimeLeft -= Time.deltaTime;

            if (TimeLeft < 0f)
                TimeLeft = 0f;

            OnTimerChanged?.Invoke(TimeLeft);

            yield return null;
        }

        IsMatchRunning = false;

        EndMatch();
    }

    private void EndMatch()
    {
        Debug.Log("Match finished");

        if (players.Count == 0)
        {
            OnMatchEnded?.Invoke(null, null);
            return;
        }

        if (players.Count == 1)
        {
            OnMatchEnded?.Invoke(players[0], null);
            return;
        }

        PlayerScore p1 = players[0];
        PlayerScore p2 = players[1];

        PlayerScore winner = null;
        PlayerScore loser = null;

        if (p1.CurrentScore > p2.CurrentScore)
        {
            winner = p1;
            loser = p2;
        }
        else if (p2.CurrentScore > p1.CurrentScore)
        {
            winner = p2;
            loser = p1;
        }

        Debug.Log(winner == null ? "DRAW" : $"Winner: {winner.displayName}");

        OnMatchEnded?.Invoke(winner, loser);
    }
}