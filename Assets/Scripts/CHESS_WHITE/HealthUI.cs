using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public bool isWitchHealthBar;
    public Image healthFill;

    private PlayerHealth playerHealth;

    void Start()
    {
        FindPlayerHealth();
    }

    void Update()
    {
        if (playerHealth == null)
            FindPlayerHealth();

        if (playerHealth == null || healthFill == null)
            return;

        healthFill.fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
    }

    void FindPlayerHealth()
    {
        PlayerHealth[] players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);

        foreach (PlayerHealth hp in players)
        {
            if (hp.isWitch == isWitchHealthBar)
            {
                playerHealth = hp;
                return;
            }
        }
    }
}