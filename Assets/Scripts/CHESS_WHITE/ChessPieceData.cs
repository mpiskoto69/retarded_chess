using UnityEngine;

public class ChessPieceData : MonoBehaviour
{
    public BossType bossType = BossType.None;

    [HideInInspector]
    public bool isBossPiece = false;
}