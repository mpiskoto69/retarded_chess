using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isWitch;

    public float maxHealth = 8f;
    public float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (GameManager.Instance == null)
            return;

        if (isWitch)
            currentHealth = GameManager.Instance.witchHealth;
        else
            currentHealth = GameManager.Instance.nunHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        if (GameManager.Instance != null)
        {
            if (isWitch)
                GameManager.Instance.SaveWitchHealth(currentHealth);
            else
                GameManager.Instance.SaveNunHealth(currentHealth);
        }

        Debug.Log(
            gameObject.name +
            " HP: " +
            currentHealth
        );

        if (currentHealth <= 0f)
            Debug.Log(gameObject.name + " died");
    }
}