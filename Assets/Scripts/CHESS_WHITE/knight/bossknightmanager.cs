using UnityEngine;
using UnityEngine.SceneManagement;

public class BossKnightManager : MonoBehaviour
{
    public static BossKnightManager Instance;

    [Header("Relic")]
    public GameObject relicPrefab;

    [Header("Nun Ring Relic Points")]
    public Transform[] nunRelicSpawnPoints;

    [Header("Witch Ring Relic Points")]
    public Transform[] witchRelicSpawnPoints;

    [Header("Return")]
    public string boardSceneName = "first";

    private GameObject allowedPlayer;
    private bool relicSpawned = false;
    private bool relicTaken = false;

    void Awake()
    {
        Instance = this;
    }

    public void BossKilled(MonoBehaviour boss, GameObject killerPlayer)
    {
        if (relicSpawned) return;

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

        Transform[] chosenPoints = GetSpawnPointsForPlayer(killerPlayer);

        if (chosenPoints == null || chosenPoints.Length == 0)
        {
            Debug.LogError("No relic spawn points assigned for winner!");
            return;
        }

        Transform point = chosenPoints[Random.Range(0, chosenPoints.Length)];

        if (point == null)
        {
            Debug.LogError("Chosen relic spawn point is null!");
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

        Debug.Log("Relic spawned in winner ring for: " + allowedPlayer.name);
    }

    Transform[] GetSpawnPointsForPlayer(GameObject player)
    {
        PlayerHealth hp = player.GetComponent<PlayerHealth>();

        if (hp != null && hp.isWitch)
            return witchRelicSpawnPoints;

        return nunRelicSpawnPoints;
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