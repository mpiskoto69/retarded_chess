using UnityEngine;
using UnityEngine.SceneManagement;

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

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}