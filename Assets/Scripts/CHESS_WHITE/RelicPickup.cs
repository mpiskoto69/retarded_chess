using UnityEngine;

public class RelicPickup : MonoBehaviour
{
    private GameObject allowedPlayer;
    private bool pickedUp = false;

    public void SetAllowedPlayer(GameObject player)
    {
        allowedPlayer = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (allowedPlayer == null) return;

        bool isAllowedPlayer =
            other.gameObject == allowedPlayer ||
            other.transform.IsChildOf(allowedPlayer.transform);

        if (!isAllowedPlayer)
            return;

        pickedUp = true;

        Debug.Log(allowedPlayer.name + " picked up relic!");

        if (BossRookManager.Instance != null)
        {
            BossRookManager.Instance.RelicTaken(allowedPlayer);
        }
        else if (BossKnightManager.Instance != null)
        {
            BossKnightManager.Instance.RelicTaken(allowedPlayer);
        }
        else if (BossFightManager.Instance != null)
        {
            BossFightManager.Instance.RelicTaken(allowedPlayer);
        }
        else
        {
            Debug.LogError("No boss manager found for relic pickup!");
        }

        Destroy(gameObject);
    }
}