using UnityEngine;

public class HealthConsumablePickup : MonoBehaviour
{
    public float healAmount = 0.5f;

    private bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        PlayerHealth hp = other.GetComponentInParent<PlayerHealth>();

        if (hp == null)
            return;

        pickedUp = true;

        hp.Heal(healAmount);

        Destroy(gameObject);
    }
}