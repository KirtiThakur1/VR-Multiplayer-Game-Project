using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [Header("References")]
    public PlayerScore playerScore;
    public PlayerScoreUI overheadUI;
    public PlayerEndGameUI endGameUI;

    private void Awake()
    {
        if (playerScore == null)
            playerScore = GetComponent<PlayerScore>();

        if (overheadUI == null)
            overheadUI = GetComponentInChildren<PlayerScoreUI>(true);

        if (endGameUI == null)
            endGameUI = GetComponentInChildren<PlayerEndGameUI>(true);
    }

    private void Start()
    {
        SetupPlayer();
    }

    public void SetupPlayer()
    {
        if (playerScore == null)
        {
            Debug.LogError($"PlayerSetup on {name}: PlayerScore not found.");
            return;
        }

        playerScore.ResetScore();

        if (overheadUI != null)
            overheadUI.Bind(playerScore);

        if (endGameUI != null)
            endGameUI.Bind(playerScore);

        if (MatchManager.Instance != null)
            MatchManager.Instance.RegisterPlayer(playerScore);

        Debug.Log($"PlayerSetup completed for {name}");
    }
}