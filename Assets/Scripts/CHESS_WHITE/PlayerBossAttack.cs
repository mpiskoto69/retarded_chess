using UnityEngine;

public class PlayerBossAttack : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode attackKey = KeyCode.Return;

    [Header("Boss")]
    public MonoBehaviour myBoss;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackAngle = 60f;
    public float attackYawOffset = 0f;

    [Header("Debug")]
    public bool drawDebug = true;

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

        if (!IsBossInFront())
        {
            Debug.Log("BOSS NOT IN FRONT");
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

    bool IsBossInFront()
    {
        Vector3 toBoss = myBoss.transform.position - transform.position;
        toBoss.y = 0f;

        if (toBoss.sqrMagnitude < 0.01f)
            return true;

        toBoss.Normalize();

        Vector3 attackForward =
            Quaternion.Euler(0f, attackYawOffset, 0f) * transform.forward;

        attackForward.y = 0f;
        attackForward.Normalize();

        float angle = Vector3.Angle(attackForward, toBoss);

        Debug.Log("Attack angle to boss = " + angle);

        return angle <= attackAngle * 0.5f;
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

    void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 attackForward =
            Quaternion.Euler(0f, attackYawOffset, 0f) * transform.forward;

        Quaternion leftRot = Quaternion.Euler(0f, -attackAngle * 0.5f, 0f);
        Quaternion rightRot = Quaternion.Euler(0f, attackAngle * 0.5f, 0f);

        Vector3 leftDir = leftRot * attackForward;
        Vector3 rightDir = rightRot * attackForward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir.normalized * attackRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDir.normalized * attackRange);
    }
}