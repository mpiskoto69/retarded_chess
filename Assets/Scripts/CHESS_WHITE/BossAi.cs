using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;

    [Header("Stats")]
    public int hitsToDie = 3;
    public float damage = 0.5f;

    [Header("Attack")]
    public float attackRange = 5f;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Animator")]
    public Animator animator;

    private NavMeshAgent agent;
    private int hitsTaken = 0;
    private float nextAttackTime = 0f;
    private bool isDead = false;

    public int CurrentHits => hitsTaken;
    public int MaxHits => hitsToDie;

  void Awake()
{
    agent = GetComponent<NavMeshAgent>();

    agent.speed = moveSpeed;
    agent.stoppingDistance = attackRange;
    agent.updateRotation = false;

    if (animator == null)
        animator = GetComponentInChildren<Animator>();

    animator.SetBool("Die", false);
}

    void Update()
    {
        if (isDead) return;
        if (targetPlayer == null) return;

        float xDifference = Mathf.Abs(transform.position.x - targetPlayer.position.x);
        float zDifference = Mathf.Abs(transform.position.z - targetPlayer.position.z);

        FaceTarget();

        if (xDifference <= attackRange && zDifference <= attackRange)
        {
            agent.isStopped = true;

            if (Time.time >= nextAttackTime)
            {
                DoAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }

        UpdateAnimator();
    }

    void DoAttack()
    {
        Debug.Log(gameObject.name + " ATTACK!");

        if (animator != null && HasParameter("Attack"))
            animator.SetTrigger("Attack");

        PlayerHealth hp = targetPlayer.GetComponent<PlayerHealth>();

        if (hp == null)
        {
            Debug.LogError("NO PlayerHealth on target: " + targetPlayer.name);
            return;
        }

        hp.TakeDamage(damage);
        Debug.Log(targetPlayer.name + " lost " + damage + " HP");
    }

    void FaceTarget()
    {
        Vector3 dir = targetPlayer.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            8f * Time.deltaTime
        );
    }

public void TakeHit(GameObject attacker)
{
    Debug.Log("BOSS GOT HIT");

    if (isDead) return;

    hitsTaken++;

    Debug.Log("HITS = " + hitsTaken);

    if (hitsTaken >= hitsToDie)
        Die(attacker);
}

    void Die(GameObject killerPlayer)
    {
        isDead = true;

        if (agent != null)
            agent.isStopped = true;

        if (animator != null && HasParameter("Die"))
            animator.SetBool("Die", true);

        if (BossFightManager.Instance != null)
            BossFightManager.Instance.BossKilled(this, killerPlayer);

        enabled = false;
    }

    void UpdateAnimator()
    {
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", speed);

        if (HasParameter("IsMoving"))
            animator.SetBool("IsMoving", speed > 0.1f && !agent.isStopped);
    }

    bool HasParameter(string parameterName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.name == parameterName)
                return true;

        return false;
    }
}