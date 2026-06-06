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

        if (other.gameObject != allowedPlayer)
            return;

        Debug.Log(other.name + " picked up relic!");

        BossFightManager.Instance.RelicTaken(other.gameObject);

        gameObject.SetActive(false);
    }
}