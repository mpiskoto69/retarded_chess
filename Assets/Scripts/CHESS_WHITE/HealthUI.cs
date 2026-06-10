using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth nunHealth;
    public PlayerHealth witchHealth;

    public TMP_Text nunText;
    public TMP_Text witchText;

    void Update()
    {
        if (nunHealth != null && nunText != null)
            nunText.text = "Nun " + Hearts(nunHealth.currentHealth);

        if (witchHealth != null && witchText != null)
            witchText.text = "Witch " + Hearts(witchHealth.currentHealth);
    }

    string Hearts(float hp)
    {
        string result = "";

        int fullHearts = Mathf.FloorToInt(hp);

        for (int i = 0; i < fullHearts; i++)
            result += "■";

        if (hp % 1f >= 0.5f)
            result += "□";

        return result;
    }
}