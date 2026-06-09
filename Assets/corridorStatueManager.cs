using UnityEngine;
using UnityEngine.AI;

public class CorridorStatueSpawner : MonoBehaviour
{
    [Header("Statue")]
    public GameObject statuePrefab;

    [Header("Manual Spawn Limits")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -20f;
    public float maxZ = 20f;
    public float spawnY = 0f;

    [Header("Spawn")]
    public int statueCount = 6;
    public Vector3 spawnOffset = Vector3.zero;
    public Vector3 spawnRotation = Vector3.zero;
    public Vector3 spawnScale = Vector3.one;

    [Header("Spacing")]
    public float minDistanceBetweenStatues = 3f;
    public int maxAttemptsPerStatue = 60;

    [Header("Navigation")]
    public bool snapToNavMesh = true;
    public float navMeshSearchRadius = 3f;

    private Vector3[] spawnedPositions;

    void Start()
    {
        SpawnStatues();
    }

    void SpawnStatues()
    {
        if (statuePrefab == null)
        {
            Debug.LogError("Statue Prefab is not assigned!");
            return;
        }

        spawnedPositions = new Vector3[statueCount];
        int spawnedCount = 0;

        for (int i = 0; i < statueCount; i++)
        {
            bool spawned = false;

            for (int attempt = 0; attempt < maxAttemptsPerStatue; attempt++)
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(minX, maxX),
                    spawnY,
                    Random.Range(minZ, maxZ)
                ) + spawnOffset;

                if (snapToNavMesh)
                {
                    NavMeshHit hit;

                    if (!NavMesh.SamplePosition(randomPosition, out hit, navMeshSearchRadius, NavMesh.AllAreas))
                        continue;

                    randomPosition = hit.position + spawnOffset;
                }

                if (!IsFarEnough(randomPosition, spawnedPositions, spawnedCount))
                    continue;

                GameObject statue = Instantiate(
                    statuePrefab,
                    randomPosition,
                    Quaternion.Euler(spawnRotation)
                );

                statue.transform.localScale = spawnScale;
                statue.SetActive(true);

                EnsureObstacleBlocksMovement(statue);

                spawnedPositions[spawnedCount] = randomPosition;
                spawnedCount++;
                spawned = true;

                break;
            }

            if (!spawned)
                Debug.LogWarning("Could not spawn statue " + i + ". Try bigger limits or smaller min distance.");
        }

        Debug.Log("Spawned " + spawnedCount + " statues.");
    }

    bool IsFarEnough(Vector3 position, Vector3[] existingPositions, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float distance = Vector2.Distance(
                new Vector2(position.x, position.z),
                new Vector2(existingPositions[i].x, existingPositions[i].z)
            );

            if (distance < minDistanceBetweenStatues)
                return false;
        }

        return true;
    }

    void EnsureObstacleBlocksMovement(GameObject statue)
    {
        Collider col = statue.GetComponent<Collider>();

        if (col == null)
        {
            BoxCollider box = statue.AddComponent<BoxCollider>();
            box.isTrigger = false;
        }
        else
        {
            col.isTrigger = false;
        }

        NavMeshObstacle obstacle = statue.GetComponent<NavMeshObstacle>();

        if (obstacle == null)
            obstacle = statue.AddComponent<NavMeshObstacle>();

        obstacle.carving = true;
        obstacle.carveOnlyStationary = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (minX + maxX) * 0.5f,
            spawnY,
            (minZ + maxZ) * 0.5f
        );

        Vector3 size = new Vector3(
            maxX - minX,
            0.1f,
            maxZ - minZ
        );

        Gizmos.DrawWireCube(center, size);
    }
}