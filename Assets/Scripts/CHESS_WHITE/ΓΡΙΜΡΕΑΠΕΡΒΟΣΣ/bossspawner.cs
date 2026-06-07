using UnityEngine;
using UnityEngine.AI;

public class BossRandomSpawner : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnSetup
    {
        public string name;
        public GameObject boss;
        public Transform[] spawnPoints;
    }

    public BossSpawnSetup[] bosses;

    void Start()
    {
        SpawnAllBosses();
    }

    void SpawnAllBosses()
    {
        foreach (BossSpawnSetup setup in bosses)
        {
            SpawnBoss(setup);
        }
    }

    void SpawnBoss(BossSpawnSetup setup)
    {
        if (setup == null)
            return;

        if (setup.boss == null)
        {
            Debug.LogError("Boss is not assigned in setup: " + setup.name);
            return;
        }

        if (setup.spawnPoints == null || setup.spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned for boss: " + setup.name);
            return;
        }

        Transform point = setup.spawnPoints[Random.Range(0, setup.spawnPoints.Length)];

        setup.boss.SetActive(true);

        NavMeshAgent agent = setup.boss.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(point.position);
        }
        else
        {
            setup.boss.transform.position = point.position;
        }

        setup.boss.transform.rotation = point.rotation;

        Debug.Log(setup.name + " spawned at: " + point.name);
    }
}