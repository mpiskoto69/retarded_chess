using TMPro;
using UnityEngine;

public class RelicCountUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text nunRelicsText;
    public TMP_Text witchRelicsText;

    [Header("Labels")]
    public string nunLabel = "Nun Relics";
    public string witchLabel = "Witch Relics";

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (GameManager.Instance == null)
        {
            if (nunRelicsText != null)
                nunRelicsText.text = nunLabel + ": 0";

            if (witchRelicsText != null)
                witchRelicsText.text = witchLabel + ": 0";

            return;
        }

        if (nunRelicsText != null)
            nunRelicsText.text = nunLabel + ": " + GameManager.Instance.nunRelics;

        if (witchRelicsText != null)
            witchRelicsText.text = witchLabel + ": " + GameManager.Instance.witchRelics;
    }
}