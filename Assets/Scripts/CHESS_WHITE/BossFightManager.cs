using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance;

    [Header("Relic")]
    public GameObject relicPrefab;
    public Transform[] relicSpawnPoints;

    [Header("Return")]
    public string boardSceneName = "first";

    private GameObject allowedPlayer;
    private bool relicSpawned = false;
    private bool relicTaken = false;

    void Awake()
    {
        Instance = this;
        Debug.Log("BossFightManager ready");
    }

    public void BossKilled(BossAI boss, GameObject killerPlayer)
    {
        Debug.Log("BossKilled called");

        if (relicSpawned)
        {
            Debug.LogWarning("Relic already spawned");
            return;
        }

        relicSpawned = true;
        allowedPlayer = killerPlayer;

        if (allowedPlayer == null)
        {
            Debug.LogError("Killer player is null!");
            return;
        }

        if (relicPrefab == null)
        {
            Debug.LogError("Relic Prefab is not assigned!");
            return;
        }

        if (relicSpawnPoints == null || relicSpawnPoints.Length == 0)
        {
            Debug.LogError("No relic spawn points assigned!");
            return;
        }

        int index = Random.Range(0, relicSpawnPoints.Length);
        Transform point = relicSpawnPoints[index];

        if (point == null)
        {
            Debug.LogError("Relic spawn point " + index + " is null!");
            return;
        }

        GameObject spawnedRelic = Instantiate(
            relicPrefab,
            point.position,
            Quaternion.Euler(-90f, 0f, 0f)
        );

        spawnedRelic.SetActive(true);

        RelicPickup pickup = spawnedRelic.GetComponent<RelicPickup>();

        if (pickup != null)
            pickup.SetAllowedPlayer(allowedPlayer);
        else
            Debug.LogError("Spawned relic has no RelicPickup script!");

        Debug.Log("Relic spawned at " + point.name + " for " + allowedPlayer.name);
    }

    public void RelicTaken(GameObject player)
    {
        if (relicTaken) return;
        if (player != allowedPlayer) return;

        relicTaken = true;

        Debug.Log(player.name + " got the relic!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRelicToPlayer(player);
            GameManager.Instance.MarkBossDefeated(GameManager.Instance.currentBoss);

            if (GameManager.Instance.IsGameFinished())
            {
                EndGameUI endUI = FindFirstObjectByType<EndGameUI>();

                if (endUI != null)
                {
                    endUI.ShowYouWon();
                    return;
                }
            }
        }

        SceneManager.LoadScene(boardSceneName);
    }
}