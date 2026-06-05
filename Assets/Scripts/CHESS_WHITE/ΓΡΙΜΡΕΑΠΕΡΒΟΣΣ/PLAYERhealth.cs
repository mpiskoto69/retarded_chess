using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 8f;
    public float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log(gameObject.name + " HP: " + currentHealth);

        if (currentHealth <= 0f)
            Debug.Log(gameObject.name + " died");
    }
}