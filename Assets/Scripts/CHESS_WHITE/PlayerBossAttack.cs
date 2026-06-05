using UnityEngine;

public class PlayerBossAttack : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Return;
    public float attackRange = 4f;
    public LayerMask bossLayer;

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
            Attack();
    }

    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            bossLayer
        );

        foreach (Collider hit in hits)
        {
            BossAI boss = hit.GetComponentInParent<BossAI>();

            if (boss != null)
            {
                boss.TakeHit(gameObject);
                break;
            }
        }
    }
}