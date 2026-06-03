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

        if (!other.CompareTag("Player"))
            return;

        if (data == null)
            return;

        if (!data.isBossPiece)
            return;

        if (data.bossType == BossType.None)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        if (GameManager.Instance.IsBossDefeated(data.bossType))
            return;

        triggered = true;

        GameManager.Instance.currentBoss = data.bossType;

        switch (data.bossType)
        {
            case BossType.Bishop:
                SceneManager.LoadScene("BISHOPGRAVEYARD");
                break;

            case BossType.Knight:
                SceneManager.LoadScene("KnightArena");
                break;

            case BossType.Rook:
                SceneManager.LoadScene("RookArena");
                break;
        }
    }
}