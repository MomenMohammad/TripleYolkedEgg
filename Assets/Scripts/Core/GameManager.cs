using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Match Settings")]
    [SerializeField] private int startingStocks = 3;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private float matchTime = 480f; // 8 minutes
    [SerializeField] private Transform[] spawnPoints;

    private float currentMatchTime;
    private Dictionary<int, int> playerStocks = new Dictionary<int, int>();
    private HashSet<int> playersRespawning = new HashSet<int>();
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!isGameOver && currentMatchTime > 0)
        {
            currentMatchTime -= Time.deltaTime;
            if (currentMatchTime <= 0)
            {
                currentMatchTime = 0;
                CheckWinCondition();
            }
        }
    }

    public void StartMatch(int playerCount)
    {
        playerStocks.Clear();
        playersRespawning.Clear();
        for (int i = 1; i <= playerCount; i++)
        {
            playerStocks[i] = startingStocks;
        }
        currentMatchTime = matchTime;
        isGameOver = false;
    }

    public float GetCurrentTime() => currentMatchTime;

    public void RestartMatch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPlayerKilled(int playerNumber, GameObject playerObject)
    {
        if (playerObject == null) return;
        if (playersRespawning.Contains(playerNumber)) return;

        // If the game is already over, just hide the player and return
        if (isGameOver)
        {
            playerObject.SetActive(false);
            return;
        }

        playersRespawning.Add(playerNumber);

        // Safety: ensure the player exists in the dictionary
        if (!playerStocks.ContainsKey(playerNumber))
        {
            playerStocks[playerNumber] = startingStocks;
        }

        playerStocks[playerNumber]--;
        Debug.Log($"Player {playerNumber} killed. Stocks left: {playerStocks[playerNumber]}");

        if (playerStocks[playerNumber] <= 0)
{
            playerObject.SetActive(false);
            CheckWinCondition();
        }
        else
        {
            StartCoroutine(RespawnSequence(playerNumber, playerObject));
        }
    }

    private IEnumerator RespawnSequence(int playerNumber, GameObject playerObject)
    {
        if (playerObject == null) yield break;

        playerObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        
        // Safety: check if object still exists after delay
        if (playerObject == null) yield break;

        // Reset player state
        FighterHealth health = playerObject.GetComponent<FighterHealth>();
        if (health != null) health.ResetDamage();

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Position at spawn point
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int spawnIndex = Mathf.Clamp(playerNumber - 1, 0, spawnPoints.Length - 1);
            playerObject.transform.position = spawnPoints[spawnIndex].position;
        }
        
        playerObject.SetActive(true);
        playersRespawning.Remove(playerNumber);
        
        Debug.Log($"Player {playerNumber} respawned.");
    }

    private void CheckWinCondition()
    {
        int playersAlive = 0;
        int winner = -1;

        foreach (var entry in playerStocks)
        {
            if (entry.Value > 0)
            {
                playersAlive++;
                winner = entry.Key;
            }
        }

        if (playersAlive <= 1)
        {
            isGameOver = true;
            Debug.Log($"Player {winner} Wins!");
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.ShowWinScreen(winner);
            }
        }
}

    public int GetStocks(int playerNumber)
    {
        return playerStocks.ContainsKey(playerNumber) ? playerStocks[playerNumber] : 0;
    }
}
