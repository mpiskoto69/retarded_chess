using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance;

    [Header("Relic")]
    public GameObject relic;
    public Transform[] relicSpawnPoints;

    private GameObject allowedPlayer;
    private bool relicSpawned = false;

    void Awake()
    {
        Instance = this;

        if (relic != null)
            relic.SetActive(false);
    }

    public void BossKilled(BossAI boss, GameObject killerPlayer)
    {
        if (relicSpawned) return;

        relicSpawned = true;
        allowedPlayer = killerPlayer;

        Transform point = relicSpawnPoints[Random.Range(0, relicSpawnPoints.Length)];

        relic.transform.position = point.position;
        relic.SetActive(true);

        RelicPickup pickup = relic.GetComponent<RelicPickup>();
        if (pickup != null)
            pickup.SetAllowedPlayer(allowedPlayer);

        Debug.Log("Relic spawned for: " + allowedPlayer.name);
    }

    public void RelicTaken(GameObject player)
    {
        if (player != allowedPlayer) return;

        Debug.Log(player.name + " got the relic!");

        // εδώ αργότερα προσθέτεις relic score
        SceneManager.LoadScene("first");
    }
}