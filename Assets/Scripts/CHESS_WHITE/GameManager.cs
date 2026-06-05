using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BossType currentBoss;

    private HashSet<BossType> defeatedBosses = new HashSet<BossType>();

    public int nunRelics = 0;
    public int witchRelics = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsBossDefeated(BossType bossType)
    {
        return defeatedBosses.Contains(bossType);
    }

    public void MarkBossDefeated(BossType bossType)
    {
        if (!defeatedBosses.Contains(bossType))
            defeatedBosses.Add(bossType);
    }

    public void AddRelicToNun()
    {
        nunRelics++;
    }

    public void AddRelicToWitch()
    {
        witchRelics++;
    }
}