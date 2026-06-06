using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isWitch;

    public float maxHealth = 8f;
    public float currentHealth;

    public bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (GameManager.Instance == null)
            return;

        currentHealth = isWitch
            ? GameManager.Instance.witchHealth
            : GameManager.Instance.nunHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (GameManager.Instance != null)
        {
            if (isWitch)
                GameManager.Instance.SaveWitchHealth(currentHealth);
            else
                GameManager.Instance.SaveNunHealth(currentHealth);
        }

        Debug.Log(gameObject.name + " HP: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " died");

        MonoBehaviour movement = GetComponent<NunMovement>();
        if (movement != null) movement.enabled = false;

        MonoBehaviour witchMovement = GetComponent<WitchMovement>();
        if (witchMovement != null) witchMovement.enabled = false;

        PlayerBossAttack attack = GetComponent<PlayerBossAttack>();
        if (attack != null) attack.enabled = false;
    }
}