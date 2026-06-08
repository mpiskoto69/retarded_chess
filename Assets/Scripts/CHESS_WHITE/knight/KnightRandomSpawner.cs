using UnityEngine;
using UnityEngine.AI;

public class KnightRandomSpawner : MonoBehaviour
{
    [Header("Knight Boss")]
    public GameObject knightBoss;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("NavMesh")]
    public float navMeshSearchRadius = 20f;

    void Start()
    {
        SpawnKnight();
    }

    void SpawnKnight()
    {
        if (knightBoss == null)
        {
            Debug.LogError("Knight Boss is not assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No knight spawn points assigned!");
            return;
        }

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (point == null)
        {
            Debug.LogError("Chosen knight spawn point is null!");
            return;
        }

        Vector3 spawnPosition = point.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(point.position, out hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }
        else
        {
            Debug.LogError("Knight spawn point is not near NavMesh: " + point.name);
            return;
        }

        NavMeshAgent agent = knightBoss.GetComponent<NavMeshAgent>();

        knightBoss.SetActive(true);

        if (agent != null)
            agent.enabled = false;

        knightBoss.transform.position = spawnPosition;
        knightBoss.transform.rotation = point.rotation;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(spawnPosition);
        }

        Debug.Log("Knight spawned at " + spawnPosition + " from " + point.name);
    }
}