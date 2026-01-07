using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Combo Settings")]
    public float comboMaxGap = 0.3f;

    [Header("Penalties")]
    public int bombPenalty = 20;
    public int butterflyPenalty = 5;

    public int CurrentScore { get; private set; }

    private int _currentComboCount = 0;
    private float _lastSliceTime = -999f;

    public event Action<string, int, int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterFruitSlice(Fruit fruit)
    {
        float now = Time.time;

        if (now - _lastSliceTime <= comboMaxGap)
            _currentComboCount++;
        else
            _currentComboCount = 1;

        _lastSliceTime = now;

        int delta = fruit.baseScore * _currentComboCount;
        CurrentScore += delta;

        OnScoreChanged?.Invoke(fruit.name, delta, CurrentScore);
    }

    public void RegisterBombSlice()
    {
        _currentComboCount = 0;
        int delta = -bombPenalty;
        CurrentScore += delta;

        OnScoreChanged?.Invoke("Bomb", delta, CurrentScore);
    }

    public void RegisterButterflyHit()
    {
        _currentComboCount = 0;
        int delta = -butterflyPenalty;
        CurrentScore += delta;

        OnScoreChanged?.Invoke("Butterfly", delta, CurrentScore);
    }
}
