using System.Collections.Generic;
using UnityEngine;

public class RandomChessBoardSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bishopPrefab;
    public GameObject knightPrefab;
    public GameObject rookPrefab;
    public GameObject pawnPrefab;
    public GameObject queenPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Counts")]
    public int bishopCount = 2;
    public int knightCount = 2;
    public int rookCount = 2;
    public int pawnCount = 4;
    public int queenCount = 1;

    [Header("Avoid Player")]
    public Transform player;
    public float avoidPlayerDistance = 12f;

    [Header("Placement")]
    public float yOffset = 0.05f;
    public float pieceScale = 1f;
    public bool randomYRotation = true;

    [Header("Solid Collider Sizes")]
    public Vector3 bishopColliderSize = new Vector3(10f, 12f, 10f);
    public Vector3 knightColliderSize = new Vector3(8f, 10f, 8f);
    public Vector3 rookColliderSize = new Vector3(10f, 12f, 10f);
    public Vector3 pawnColliderSize = new Vector3(8f, 10f, 8f);
    public Vector3 queenColliderSize = new Vector3(12f, 14f, 12f);

    [Header("Rotation Per Piece")]
    public Vector3 bishopRotationFix = new Vector3(-90f, 0f, 0f);
    public Vector3 knightRotationFix = new Vector3(0f, 0f, 0f);
    public Vector3 rookRotationFix = new Vector3(-90f, 0f, 0f);
    public Vector3 pawnRotationFix = new Vector3(-90f, 0f, 0f);
    public Vector3 queenRotationFix = new Vector3(-90f, 0f, 0f);

    private readonly List<Transform> availablePoints = new List<Transform>();
    private readonly List<GameObject> spawnedPieces = new List<GameObject>();

    void Start()
    {
        SpawnAllPieces();
    }

    void SpawnAllPieces()
    {
        availablePoints.Clear();
        spawnedPieces.Clear();

        foreach (Transform p in spawnPoints)
        {
            if (p != null)
                availablePoints.Add(p);
        }

        List<GameObject> bishops = SpawnPieceGroup(bishopPrefab, bishopCount, BossType.Bishop, bishopRotationFix, bishopColliderSize);
        List<GameObject> knights = SpawnPieceGroup(knightPrefab, knightCount, BossType.Knight, knightRotationFix, knightColliderSize);
        List<GameObject> rooks = SpawnPieceGroup(rookPrefab, rookCount, BossType.Rook, rookRotationFix, rookColliderSize);

        MakeOneRandomBoss(bishops);
        MakeOneRandomBoss(knights);
        MakeOneRandomBoss(rooks);

        SpawnPieceGroup(pawnPrefab, pawnCount, BossType.None, pawnRotationFix, pawnColliderSize);
        SpawnPieceGroup(queenPrefab, queenCount, BossType.None, queenRotationFix, queenColliderSize);
    }

    List<GameObject> SpawnPieceGroup(GameObject prefab, int count, BossType type, Vector3 rotationFix, Vector3 colliderSize)
    {
        List<GameObject> group = new List<GameObject>();

        if (prefab == null)
            return group;

        for (int i = 0; i < count; i++)
        {
            Transform point = GetValidSpawnPoint();

            if (point == null)
                break;

            GameObject root = new GameObject(prefab.name + "_Spawned");
            root.transform.position = point.position;
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            ChessPieceData data = root.AddComponent<ChessPieceData>();
            data.bossType = type;
            data.isBossPiece = false;

            BoxCollider solid = root.AddComponent<BoxCollider>();
            solid.isTrigger = false;
            solid.center = new Vector3(0f, colliderSize.y / 2f, 0f);
            solid.size = colliderSize;

            float yRot = randomYRotation ? Random.Range(0f, 360f) : 0f;

            Quaternion visualRotation = Quaternion.Euler(
                rotationFix.x,
                rotationFix.y + yRot,
                rotationFix.z
            );

            GameObject visual = Instantiate(prefab, root.transform);
            visual.name = "Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = visualRotation;
            visual.transform.localScale = prefab.transform.localScale * pieceScale;
            RemoveCollidersAndTriggersFromVisual(visual);
            PutVisualOnBoard(root, visual, point.position);

            EnsureSolidObstacle(root);

            spawnedPieces.Add(root);
            group.Add(root);
        }

        return group;
    }

    Transform GetValidSpawnPoint()
    {
        while (availablePoints.Count > 0)
        {
            int index = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[index];
            availablePoints.RemoveAt(index);

            if (point == null)
                continue;

            if (player != null)
            {
                float distanceFromPlayer = Vector3.Distance(player.position, point.position);

                if (distanceFromPlayer < avoidPlayerDistance)
                    continue;
            }

            bool occupied = false;

            foreach (GameObject piece in spawnedPieces)
            {
                if (piece == null) continue;

                float distance = Vector3.Distance(piece.transform.position, point.position);

                if (distance < 6f)
                {
                    occupied = true;
                    break;
                }
            }

            if (occupied)
                continue;

            return point;
        }

        return null;
    }

    void RemoveCollidersAndTriggersFromVisual(GameObject visual)
    {
        ChessPieceTrigger[] triggers = visual.GetComponentsInChildren<ChessPieceTrigger>();

        foreach (ChessPieceTrigger t in triggers)
            Destroy(t);

        Collider[] colliders = visual.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
            Destroy(c);
    }

    void PutVisualOnBoard(GameObject root, GameObject visual, Vector3 boardPosition)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            root.transform.position = boardPosition + Vector3.up * yOffset;
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        float lift = boardPosition.y + yOffset - bounds.min.y;
        root.transform.position += Vector3.up * lift;
    }

    void MakeOneRandomBoss(List<GameObject> group)
    {
        if (group == null || group.Count == 0)
            return;

        int index = Random.Range(0, group.Count);
        GameObject bossPiece = group[index];

        ChessPieceData data = bossPiece.GetComponent<ChessPieceData>();

        if (data == null)
            data = bossPiece.AddComponent<ChessPieceData>();

        data.isBossPiece = true;

        AddInteractionTrigger(bossPiece);
    }
void EnsureSolidObstacle(GameObject obj)
{
    BoxCollider col = obj.GetComponent<BoxCollider>();

    if (col == null)
        col = obj.AddComponent<BoxCollider>();

    col.isTrigger = false;

    Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

    if (renderers.Length == 0)
    {
        col.center = new Vector3(0f, -10f, 0f);
        col.size = new Vector3(10f, 20f, 10f);
        return;
    }

    Bounds bounds = renderers[0].bounds;

    for (int i = 1; i < renderers.Length; i++)
        bounds.Encapsulate(renderers[i].bounds);

    Vector3 localCenter = obj.transform.InverseTransformPoint(bounds.center);
    Vector3 localSize = obj.transform.InverseTransformVector(bounds.size);

    localSize.x = Mathf.Abs(localSize.x);
    localSize.y = Mathf.Abs(localSize.y);
    localSize.z = Mathf.Abs(localSize.z);

    col.center = localCenter;

    col.size = new Vector3(
        localSize.x * 0.9f,
        localSize.y * 0.95f,
        localSize.z * 0.9f
    );
}
    void AddInteractionTrigger(GameObject obj)
    {
        BoxCollider solid = obj.GetComponent<BoxCollider>();

        if (solid == null)
            return;

        GameObject triggerObject = new GameObject("BossTrigger");
        triggerObject.transform.SetParent(obj.transform);
        triggerObject.transform.localPosition = Vector3.zero;
        triggerObject.transform.localRotation = Quaternion.identity;
        triggerObject.transform.localScale = Vector3.one;

        BoxCollider trigger = triggerObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;

        trigger.center = solid.center;
        trigger.size = solid.size * 1.15f;

        triggerObject.AddComponent<ChessPieceTrigger>();
    }
}