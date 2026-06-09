using UnityEngine;
using UnityEngine.AI;

public class CorridorStatueSpawner : MonoBehaviour
{
    [Header("Statue")]
    public GameObject statuePrefab;

    [Header("Corridor Area")]
    public Collider corridorArea;

    [Header("Spawn")]
    public int statueCount = 6;
    public float spawnHeightOffset = 0.05f;
    public Vector3 spawnOffset = Vector3.zero;
    public Vector3 spawnRotation = Vector3.zero;
    public Vector3 spawnScale = Vector3.one;

    [Header("Placement")]
    public float edgePadding = 2f;
    public float sidePadding = 1.5f;
    public float minDistanceBetweenStatues = 3f;
    public int maxAttemptsPerStatue = 40;

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

        if (corridorArea == null)
        {
            Debug.LogError("Corridor Area is not assigned!");
            return;
        }

        spawnedPositions = new Vector3[statueCount];

        Bounds bounds = corridorArea.bounds;
        int spawnedCount = 0;

        for (int i = 0; i < statueCount; i++)
        {
            bool spawned = false;

            for (int attempt = 0; attempt < maxAttemptsPerStatue; attempt++)
            {
                Vector3 randomPosition = GetRandomPointInCorridor(bounds) + spawnOffset;

                if (!IsFarEnough(randomPosition, spawnedCount))
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
                Debug.LogWarning("Could not spawn statue " + i);
        }

        Debug.Log("Spawned " + spawnedCount + " statues in corridor.");
    }

    Vector3 GetRandomPointInCorridor(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x + sidePadding, bounds.max.x - sidePadding);
        float z = Random.Range(bounds.min.z + edgePadding, bounds.max.z - edgePadding);
        float y = bounds.max.y + spawnHeightOffset;

        return new Vector3(x, y, z);
    }

    bool IsFarEnough(Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Vector3.Distance(position, spawnedPositions[i]) < minDistanceBetweenStatues)
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
}