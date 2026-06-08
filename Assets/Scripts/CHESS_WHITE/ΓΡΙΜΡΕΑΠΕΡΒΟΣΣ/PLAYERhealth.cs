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

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
            Die();
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
        if (isDead) return;

        isDead = true;
        Debug.Log(gameObject.name + " died");

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        PlayerBossAttack attack = GetComponent<PlayerBossAttack>();
        if (attack != null)
            attack.enabled = false;

        GameOverUI gameOver = FindFirstObjectByType<GameOverUI>();
        if (gameOver != null)
            gameOver.ShowLoser();
    }
  public void Heal(float amount)
{
    if (isDead) return;

    currentHealth += amount;
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    if (GameManager.Instance != null)
    {
        if (isWitch)
            GameManager.Instance.SaveWitchHealth(currentHealth);
        else
            GameManager.Instance.SaveNunHealth(currentHealth);
    }

    Debug.Log(gameObject.name + " healed. HP: " + currentHealth);
}
}