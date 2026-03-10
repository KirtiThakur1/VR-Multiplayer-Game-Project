using UnityEngine;
using System;

public class PlayerScore : MonoBehaviour
{
    [Header("Player Info")]
    public string displayName = "Player";

    [Header("Combo Settings")]
    public float comboMaxGap = 0.3f;

    [Header("Penalties")]
    public int bombPenalty = 20;
    public int butterflyPenalty = 5;

    public int CurrentScore { get; private set; }
    public string LastObjectName { get; private set; } = "-";
    public int LastDelta { get; private set; } = 0;

    private int currentComboCount = 0;
    private float lastSliceTime = -999f;

    public event Action<PlayerScore> OnScoreChanged;

    public void ResetScore()
    {
        CurrentScore = 0;
        LastObjectName = "-";
        LastDelta = 0;
        currentComboCount = 0;
        lastSliceTime = -999f;

        OnScoreChanged?.Invoke(this);
    }

    public void RegisterFruitSlice(Fruit fruit)
    {
        float now = Time.time;

        if (now - lastSliceTime <= comboMaxGap)
            currentComboCount++;
        else
            currentComboCount = 1;

        lastSliceTime = now;

        int delta = fruit.baseScore * currentComboCount;
        CurrentScore += delta;

        LastObjectName = fruit.name.Replace("(Clone)", "").Trim();
        LastDelta = delta;

        OnScoreChanged?.Invoke(this);
    }

    public void RegisterBombSlice()
    {
        currentComboCount = 0;

        int delta = -bombPenalty;
        CurrentScore += delta;

        LastObjectName = "Bomb";
        LastDelta = delta;

        OnScoreChanged?.Invoke(this);
    }

    public void RegisterButterflyHit()
    {
        currentComboCount = 0;

        int delta = -butterflyPenalty;
        CurrentScore += delta;

        LastObjectName = "Butterfly";
        LastDelta = delta;

        OnScoreChanged?.Invoke(this);
    }
}