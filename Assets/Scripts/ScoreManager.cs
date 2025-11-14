using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Combo Settings")]
    public float comboMaxGap = 0.3f; 

    public int CurrentScore { get; private set; }

    private int _currentComboCount = 0;
    private float _lastSliceTime = -999f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterSlice(Fruit fruit)
    {
        float now = Time.time;

        if (now - _lastSliceTime <= comboMaxGap)
        {
            _currentComboCount++;
        }
        else
        {
            _currentComboCount = 1;
        }

        _lastSliceTime = now;

        int pointsToAdd = fruit.baseScore * _currentComboCount;

        CurrentScore += pointsToAdd;

        Debug.Log($"Slice! Combo = {_currentComboCount}, +{pointsToAdd} points, Total = {CurrentScore}");
    }
}
