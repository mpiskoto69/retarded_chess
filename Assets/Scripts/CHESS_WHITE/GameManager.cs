using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Boss")]
    public BossType currentBoss;

    private HashSet<BossType> defeatedBosses = new HashSet<BossType>();

    [Header("Relics")]
    public int nunRelics = 0;
    public int witchRelics = 0;
    public int relicsNeededToEndGame = 2;

    [Header("Health")]
    public float maxHealth = 8f;
    public float nunHealth = 8f;
    public float witchHealth = 8f;

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

    public void SaveNunHealth(float hp)
    {
        nunHealth = Mathf.Clamp(hp, 0f, maxHealth);
    }

    public void SaveWitchHealth(float hp)
    {
        witchHealth = Mathf.Clamp(hp, 0f, maxHealth);
    }

    public bool IsBossDefeated(BossType bossType)
    {
        return defeatedBosses.Contains(bossType);
    }

    public void MarkBossDefeated(BossType bossType)
    {
        defeatedBosses.Add(bossType);
    }

    public void AddRelicToPlayer(GameObject player)
    {
        PlayerHealth hp = player.GetComponent<PlayerHealth>();

        if (hp != null && hp.isWitch)
            witchRelics++;
        else
            nunRelics++;
    }

    public int GetTotalRelics()
    {
        return nunRelics + witchRelics;
    }

    public bool IsGameFinished()
    {
        return GetTotalRelics() >= relicsNeededToEndGame;
    }

    public string GetWinnerText()
    {
        if (nunRelics > witchRelics)
            return "Nun wins!";

        if (witchRelics > nunRelics)
            return "Witch wins!";

        return "Draw!";
    }

    public void ResetGame()
    {
        nunRelics = 0;
        witchRelics = 0;
        nunHealth = maxHealth;
        witchHealth = maxHealth;
        defeatedBosses.Clear();
    }
}