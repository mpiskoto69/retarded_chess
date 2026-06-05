using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessPieceTrigger : MonoBehaviour
{
    private ChessPieceData data;
    private bool triggered = false;

    void Awake()
    {
        data = GetComponentInParent<ChessPieceData>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        if (data == null) return;
        if (!data.isBossPiece) return;
        if (data.bossType == BossType.None) return;

        if (GameManager.Instance != null &&
            GameManager.Instance.IsBossDefeated(data.bossType))
            return;

        triggered = true;

        if (GameManager.Instance != null)
            GameManager.Instance.currentBoss = data.bossType;

        if (data.bossType == BossType.Bishop)
            SceneManager.LoadScene("BISHOPGARVEYARD");

        if (data.bossType == BossType.Knight)
            SceneManager.LoadScene("KnightArena");

        if (data.bossType == BossType.Rook)
            SceneManager.LoadScene("RookArena");
    }
}