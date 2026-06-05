using UnityEngine;

public class RelicPickup : MonoBehaviour
{
    private GameObject allowedPlayer;

    public void SetAllowedPlayer(GameObject player)
    {
        allowedPlayer = player;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.gameObject != allowedPlayer)
        {
            Debug.Log(other.name + " cannot pick this relic.");
            return;
        }

        BossFightManager.Instance.RelicTaken(other.gameObject);
    }
}