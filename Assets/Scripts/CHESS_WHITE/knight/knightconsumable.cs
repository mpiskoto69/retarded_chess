using UnityEngine;

public class KnightConsumableSpawner : MonoBehaviour
{
    [Header("Nun Consumable")]
    public GameObject nunConsumablePrefab;

    [Header("Witch Consumable")]
    public GameObject witchConsumablePrefab;

    [Header("Ring Areas")]
    public Collider nunRingArea;
    public Collider witchRingArea;

    [Header("Spawn")]
    public float spawnHeightOffset = 0.2f;
    public Vector3 spawnRotation = new Vector3(0f, 180f, 0f);
    public Vector3 spawnScale = new Vector3(10f, 10f, 10f);

    void Start()
    {
        SpawnInArea(nunConsumablePrefab, nunRingArea, "Nun Ring");
        SpawnInArea(witchConsumablePrefab, witchRingArea, "Witch Ring");
    }

    void SpawnInArea(GameObject prefab, Collider area, string ringName)
    {
        if (prefab == null)
        {
            Debug.LogError("Consumable Prefab is not assigned for " + ringName);
            return;
        }

        if (area == null)
        {
            Debug.LogError("Ring area is not assigned for " + ringName);
            return;
        }

        Bounds bounds = area.bounds;

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.max.y + spawnHeightOffset,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        GameObject spawned = Instantiate(
            prefab,
            randomPosition,
            Quaternion.Euler(spawnRotation)
        );

        spawned.transform.localScale = spawnScale;
        spawned.SetActive(true);

        Debug.Log("Consumable spawned in " + ringName + " at " + randomPosition);
    }
}