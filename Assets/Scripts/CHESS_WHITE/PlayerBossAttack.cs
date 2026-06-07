using UnityEngine;

public class PlayerBossAttack : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode attackKey = KeyCode.Return;

    [Header("Boss")]
    public BossAI myBoss;

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

        float distance = Vector3.Distance(transform.position, myBoss.transform.position);

        Debug.Log("Distance to " + myBoss.name + " = " + distance);

        if (distance > attackRange)
        {
            Debug.Log("TOO FAR");
            return;
        }

        Debug.Log(name + " HIT " + myBoss.name);

        myBoss.TakeHit(gameObject);
    }

    public void MarkBossDefeated()
    {
        bossDefeated = true;
    }
}