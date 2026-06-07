using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    public Transform targetPlayer;

    [Header("Health")]
    public int hitsToDie = 3;

    [Header("Attack")]
    public float damage = 0.5f;
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Death")]
    public float disappearDelay = 1.5f;

    [Header("Animator")]
    public Animator animator;

    private NavMeshAgent agent;
    private int hitsTaken = 0;
    private float nextAttackTime = 0f;
    private bool isDead = false;
    private bool deathHandled = false;

    public int CurrentHits => hitsTaken;
    public int MaxHits => hitsToDie;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = 0f;
        agent.updateRotation = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        if (animator != null && HasParameter("Die"))
            animator.SetBool("Die", false);
    }

    void Update()
    {
        if (isDead || targetPlayer == null) return;

        float distance = GetFlatDistanceToTarget();

        FaceTarget();

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        UpdateAnimator();
    }

    float GetFlatDistanceToTarget()
    {
        if (targetPlayer == null) return Mathf.Infinity;

        Vector2 bossPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos = new Vector2(targetPlayer.position.x, targetPlayer.position.z);

        return Vector2.Distance(bossPos, playerPos);
    }

    void Attack()
    {
        if (isDead || targetPlayer == null) return;

        Debug.Log(name + " ATTACK");

        if (animator != null && HasParameter("Attack"))
        {
            Debug.Log(name + " ATTACK TRIGGER SENT");
            animator.SetTrigger("Attack");
        }
        else
        {
            Debug.LogWarning(name + " has no Attack trigger or animator missing");
        }

        PlayerHealth hp = targetPlayer.GetComponent<PlayerHealth>();

        if (hp != null)
            hp.TakeDamage(damage);
        else
            Debug.LogError("Target has no PlayerHealth: " + targetPlayer.name);
    }

    public void TakeHit(GameObject attacker)
    {
        if (isDead) return;

        hitsTaken++;

        Debug.Log(name + " HIT " + hitsTaken + "/" + hitsToDie);

        if (hitsTaken >= hitsToDie)
            Die(attacker);
    }

    void Die(GameObject killerPlayer)
    {
        if (deathHandled) return;

        isDead = true;
        deathHandled = true;

        if (killerPlayer != null)
        {
            Animator killerAnimator = killerPlayer.GetComponentInChildren<Animator>();

            if (killerAnimator != null && HasAnimatorParameter(killerAnimator, "Dance"))
                killerAnimator.SetTrigger("Dance");

            PlayerBossAttack attack = killerPlayer.GetComponent<PlayerBossAttack>();
            if (attack != null)
                attack.MarkBossDefeated();
        }

        if (agent != null)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            agent.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        if (animator != null)
        {
            if (HasParameter("IsMoving"))
                animator.SetBool("IsMoving", false);

            if (HasParameter("Speed"))
                animator.SetFloat("Speed", 0f);

            if (HasParameter("Die"))
                animator.SetBool("Die", true);
        }

        if (BossFightManager.Instance != null)
            BossFightManager.Instance.BossKilled(this, killerPlayer);

        Destroy(gameObject, disappearDelay);
    }

    void FaceTarget()
    {
        if (targetPlayer == null) return;

        Vector3 dir = targetPlayer.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            8f * Time.deltaTime
        );
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = 0f;

        if (agent != null && agent.enabled)
            speed = agent.velocity.magnitude;

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", speed);

        if (HasParameter("IsMoving"))
            animator.SetBool("IsMoving", speed > 0.1f);
    }

    bool HasParameter(string parameterName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == parameterName)
                return true;
        }

        return false;
    }

    bool HasAnimatorParameter(Animator targetAnimator, string parameterName)
    {
        if (targetAnimator == null) return false;

        foreach (AnimatorControllerParameter p in targetAnimator.parameters)
        {
            if (p.name == parameterName)
                return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}