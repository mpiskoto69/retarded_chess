using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRookManager : MonoBehaviour
{
    public static BossRookManager Instance;

    [Header("Relic")]
    public GameObject relicPrefab;

    [Header("Corridor Areas")]
    public Collider nunCorridorArea;
    public Collider witchCorridorArea;

    [Header("Relic Spawn")]
    public float relicHeightOffset = 0.2f;
    public Vector3 relicRotation = new Vector3(-90f, 0f, 0f);
    public Vector3 relicScale = Vector3.one;

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

        Collider chosenArea = GetAreaForPlayer(killerPlayer);

        if (chosenArea == null)
        {
            Debug.LogError("No corridor area assigned for winner!");
            return;
        }

        Vector3 spawnPosition = GetRandomPointInArea(chosenArea);

        GameObject spawnedRelic = Instantiate(
            relicPrefab,
            spawnPosition,
            Quaternion.Euler(relicRotation)
        );

        spawnedRelic.transform.localScale = relicScale;
        spawnedRelic.SetActive(true);

        RelicPickup pickup = spawnedRelic.GetComponent<RelicPickup>();

        if (pickup != null)
            pickup.SetAllowedPlayer(allowedPlayer);
        else
            Debug.LogError("Spawned relic has no RelicPickup script!");

        Debug.Log("Rook relic spawned for: " + allowedPlayer.name + " at " + spawnPosition);
    }

    Collider GetAreaForPlayer(GameObject player)
    {
        PlayerHealth hp = player.GetComponent<PlayerHealth>();

        if (hp != null && hp.isWitch)
            return witchCorridorArea;

        return nunCorridorArea;
    }

    Vector3 GetRandomPointInArea(Collider area)
    {
        Bounds bounds = area.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y + relicHeightOffset;

        return new Vector3(x, y, z);
    }

    public void RelicTaken(GameObject player)
    {
        if (relicTaken) return;
        if (player != allowedPlayer) return;

        relicTaken = true;

        Debug.Log(player.name + " got the rook relic!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRelicToPlayer(player);
            GameManager.Instance.MarkBossDefeated(GameManager.Instance.currentBoss);
        }

        SceneManager.LoadScene(boardSceneName);
    }
}