using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool bishopDefeated;
    public bool knightDefeated;
    public bool rookDefeated;

    public BossType currentBoss = BossType.None;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsBossDefeated(BossType type)
    {
        if (type == BossType.Bishop) return bishopDefeated;
        if (type == BossType.Knight) return knightDefeated;
        if (type == BossType.Rook) return rookDefeated;

        return false;
    }

    public void DefeatBoss(BossType type)
    {
        if (type == BossType.Bishop) bishopDefeated = true;
        if (type == BossType.Knight) knightDefeated = true;
        if (type == BossType.Rook) rookDefeated = true;
    }
}