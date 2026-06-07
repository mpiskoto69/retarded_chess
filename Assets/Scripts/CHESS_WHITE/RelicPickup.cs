using UnityEngine;

public class RelicPickup : MonoBehaviour
{
    private GameObject allowedPlayer;

    public void SetAllowedPlayer(GameObject player)
    {
        allowedPlayer = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (allowedPlayer == null)
            return;

        bool isAllowedPlayer =
            other.gameObject == allowedPlayer ||
            other.transform.IsChildOf(allowedPlayer.transform);

        if (!isAllowedPlayer)
            return;

        gameObject.SetActive(false);

        if (BossFightManager.Instance != null)
            BossFightManager.Instance.RelicTaken(allowedPlayer);
    }
}