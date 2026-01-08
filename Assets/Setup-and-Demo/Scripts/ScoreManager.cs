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

        // (تغییر 1) اگر Scene عوض می‌کنی، امتیاز حفظ می‌شود
        // اگر نمی‌خوای، می‌تونی این خط رو پاک کنی
        DontDestroyOnLoad(gameObject);

        // (تغییر 2 - اختیاری) ریست امتیاز در شروع
        CurrentScore = 0;
        _currentComboCount = 0;
        _lastSliceTime = -999f;

        Debug.Log("ScoreManager Awake: Instance set.");
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

        Debug.Log($"Score +{delta} (combo {_currentComboCount}) total={CurrentScore} fruit={fruit.name}");

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
