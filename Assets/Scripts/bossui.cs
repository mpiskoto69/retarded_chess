using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public MonoBehaviour boss;
    public TMP_Text bossText;
    public string bossName = "Boss";

    void Update()
    {
        if (boss == null || bossText == null)
            return;

        int maxHits = 0;
        int currentHits = 0;

        if (boss is BossAI normalBoss)
        {
            maxHits = normalBoss.MaxHits;
            currentHits = normalBoss.CurrentHits;
        }
        else if (boss is KnightBossAI knightBoss)
        {
            maxHits = knightBoss.MaxHits;
            currentHits = knightBoss.CurrentHits;
        }
        else if (boss is RookBossAI rookBoss)
        {
            maxHits = rookBoss.MaxHits;
            currentHits = rookBoss.CurrentHits;
        }
        else
        {
            bossText.text = bossName;
            return;
        }

        int remaining = maxHits - currentHits;

        string circles = "";

        for (int i = 0; i < maxHits; i++)
        {
            if (i < remaining)
                circles += "●";
            else
                circles += "○";
        }

        bossText.text = bossName + " " + circles;
    }
}