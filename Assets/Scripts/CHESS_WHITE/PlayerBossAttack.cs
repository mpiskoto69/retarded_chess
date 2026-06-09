using UnityEngine;

public class PlayerBossAttack : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode attackKey = KeyCode.Return;

    [Header("Boss")]
    public MonoBehaviour myBoss;

    [Header("Attack")]
    public float attackRange = 2f;

    private bool bossDefeated = false;

    void Update()
    {
        if (bossDefeated) return;

        if (Input.GetKeyDown(attackKey))
            Attack();
    }

    void Attack()
    {
        if (myBoss == null)
        {
            bossDefeated = true;
            return;
        }

        if (!myBoss.gameObject.activeInHierarchy)
        {
            bossDefeated = true;
            return;
        }

        float distance = GetFlatDistanceToBoss();

        Debug.Log("Distance to " + myBoss.name + " = " + distance);

        if (distance > attackRange)
        {
            Debug.Log("TOO FAR");
            return;
        }

        Debug.Log(name + " HIT " + myBoss.name);

        if (myBoss is BossAI normalBoss)
        {
            normalBoss.TakeHit(gameObject);
        }
        else if (myBoss is KnightBossAI knightBoss)
        {
            knightBoss.TakeHit(gameObject);
        }
        else if (myBoss is RookBossAI rookBoss)
        {
            rookBoss.TakeHit(gameObject);
        }
        else
        {
            Debug.LogError(myBoss.name + " is not BossAI, KnightBossAI, or RookBossAI!");
        }
    }

    float GetFlatDistanceToBoss()
    {
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 bossPos = new Vector2(myBoss.transform.position.x, myBoss.transform.position.z);

        return Vector2.Distance(playerPos, bossPos);
    }

    public void MarkBossDefeated()
    {
        bossDefeated = true;
    }
}