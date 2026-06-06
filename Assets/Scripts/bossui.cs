using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public BossAI boss;
    public TMP_Text bossText;
    public string bossName = "Boss";

    void Update()
    {
        if (boss == null || bossText == null)
            return;

        int remaining = boss.MaxHits - boss.CurrentHits;

        string circles = "";

        for (int i = 0; i < boss.MaxHits; i++)
        {
            if (i < remaining)
                circles += "●";
            else
                circles += "○";
        }

        bossText.text = bossName + " " + circles;
    }
}