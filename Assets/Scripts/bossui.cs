using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public BossAI boss;
    public TMP_Text bossText;

    void Update()
    {
        if (boss == null)
            return;

        bossText.text = CreateBossBars();
    }

    string CreateBossBars()
    {
        string result = "";

        int remaining = boss.MaxHits - boss.CurrentHits;

        for (int i = 0; i < boss.MaxHits; i++)
        {
            if (i < remaining)
                result += "●";
            else
                result += "○";
        }

        return result;
    }
}