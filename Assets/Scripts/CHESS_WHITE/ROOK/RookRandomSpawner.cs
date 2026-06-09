using UnityEngine;
using UnityEngine.AI;

public class RookRandomSpawner : MonoBehaviour
{
    [Header("Rook Boss")]
    public GameObject rookBoss;

    [Header("Corridor Area")]
    public Collider corridorArea;

    [Header("Spawn")]
    public float spawnHeightOffset = 0.1f;
    public Vector3 spawnRotation = Vector3.zero;

    [Header("NavMesh")]
    public float navMeshSearchRadius = 10f;

    void Start()
    {
        SpawnRook();
    }

    void SpawnRook()
    {
        if (rookBoss == null)
        {
            Debug.LogError("Rook Boss is not assigned!");
            return;
        }

        if (corridorArea == null)
        {
            Debug.LogError("Corridor Area is not assigned!");
            return;
        }

        Bounds bounds = corridorArea.bounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.max.y + spawnHeightOffset,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            randomPosition = hit.position;
        }
        else
        {
            Debug.LogError("Could not find NavMesh near random rook spawn position!");
            return;
        }

        NavMeshAgent agent = rookBoss.GetComponent<NavMeshAgent>();

        rookBoss.SetActive(true);

        if (agent != null)
            agent.enabled = false;

        rookBoss.transform.position = randomPosition;
        rookBoss.transform.rotation = Quaternion.Euler(spawnRotation);

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(randomPosition);
        }

        Debug.Log("Rook spawned at " + randomPosition);
    }
}