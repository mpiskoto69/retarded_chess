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
    Debug.Log("PLAYER ATTACK");

    Collider[] hits = Physics.OverlapSphere(
        transform.position,
        attackRange,
        bossLayer
    );

    Debug.Log("FOUND " + hits.Length + " COLLIDERS");

    foreach (Collider hit in hits)
    {
        BossAI boss = hit.GetComponentInParent<BossAI>();

        if (boss != null)
        {
            Debug.Log("HIT BOSS");
            boss.TakeHit(gameObject);
            break;
        }
    }
}
}