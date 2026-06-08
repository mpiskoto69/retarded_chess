using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KnightBossAI : MonoBehaviour
{
    public Transform targetPlayer;

    [Header("Health")]
    public int hitsToDie = 3;

    [Header("Attack")]
    public float damage = 0.5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float stoppingDistance = 1f;
    public float turnSpeed = 8f;
    public float modelYawOffset = 0f;

    [Header("Dodge")]
    public float dodgeChance = 0.35f;
    public float dodgeCooldown = 1.5f;
    public float dodgeDistance = 2f;

    [Header("Death")]
    public float disappearDelay = 1.5f;

    [Header("Animator")]
    public Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip horseAttackSound;
    public AudioClip horseDeathSound;

    private NavMeshAgent agent;
    private int hitsTaken = 0;
    private float nextAttackTime = 0f;
    private float nextDodgeTime = 0f;
    private bool isDead = false;
    private bool deathHandled = false;

    public int CurrentHits => hitsTaken;
    public int MaxHits => hitsToDie;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (animator != null && HasParameter("Die"))
            animator.SetBool("Die", false);
    }

    void Update()
    {
        if (isDead || targetPlayer == null) return;

        float distance = GetFlatDistanceToTarget();

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }

        FaceMovementOrTarget();

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        UpdateAnimator();
    }

    void Attack()
    {
        if (isDead || targetPlayer == null) return;

        Debug.Log(name + " KNIGHT ATTACK");

        if (audioSource != null && horseAttackSound != null)
            audioSource.PlayOneShot(horseAttackSound);

        if (animator != null && HasParameter("Attack"))
            animator.SetTrigger("Attack");

        PlayerHealth hp = targetPlayer.GetComponent<PlayerHealth>();

        if (hp != null)
            hp.TakeDamage(damage);
        else
            Debug.LogError("Target has no PlayerHealth: " + targetPlayer.name);
    }

    public void TakeHit(GameObject attacker)
    {
        if (isDead) return;

        if (TryDodge(attacker))
        {
            Debug.Log(name + " DODGED!");
            return;
        }

        hitsTaken++;

        Debug.Log(name + " HIT " + hitsTaken + "/" + hitsToDie);

        if (hitsTaken >= hitsToDie)
            Die(attacker);
    }

    bool TryDodge(GameObject attacker)
    {
        if (Time.time < nextDodgeTime)
            return false;

        if (Random.value > dodgeChance)
            return false;

        nextDodgeTime = Time.time + dodgeCooldown;

        if (animator != null && HasParameter("Dodge"))
            animator.SetTrigger("Dodge");

        DodgeAway(attacker);

        return true;
    }

    void DodgeAway(GameObject attacker)
    {
        if (attacker == null) return;

        Vector3 away = transform.position - attacker.transform.position;
        away.y = 0f;

        if (away.sqrMagnitude < 0.01f)
            away = -transform.forward;

        away.Normalize();

        Vector3 dodgeTarget = transform.position + away * dodgeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(dodgeTarget, out hit, 3f, NavMesh.AllAreas))
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.Warp(hit.position);
            else
                transform.position = hit.position;
        }
    }

    void Die(GameObject killerPlayer)
    {
        if (deathHandled) return;

        isDead = true;
        deathHandled = true;

        if (audioSource != null && horseDeathSound != null)
            audioSource.PlayOneShot(horseDeathSound);

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

        if (BossKnightManager.Instance != null)
            BossKnightManager.Instance.BossKilled(this, killerPlayer);
        else
            Debug.LogError("No BossKnightManager Instance found!");

        Destroy(gameObject, disappearDelay);
    }

    void FaceMovementOrTarget()
    {
        Vector3 dir = Vector3.zero;

        if (agent != null && agent.enabled && agent.velocity.sqrMagnitude > 0.05f)
            dir = agent.velocity;
        else if (targetPlayer != null)
            dir = targetPlayer.position - transform.position;

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation =
            Quaternion.LookRotation(dir) *
            Quaternion.Euler(0f, modelYawOffset, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    float GetFlatDistanceToTarget()
    {
        Vector2 bossPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos = new Vector2(targetPlayer.position.x, targetPlayer.position.z);

        return Vector2.Distance(bossPos, playerPos);
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