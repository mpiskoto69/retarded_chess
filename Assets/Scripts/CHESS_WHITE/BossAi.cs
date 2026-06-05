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

    [Header("Movement")]
    public float chaseDistance = 20f;
    public float attackDistance = 3f;
    public float diagonalOffset = 3f;
    public float strafeSpeed = 2f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;

    [Header("Animator")]
    public Animator animator;

    private NavMeshAgent agent;
    private int hitsTaken = 0;
    private float nextAttackTime;
    private int strafeSide = 1;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        strafeSide = Random.value > 0.5f ? 1 : -1;
    }

    void Update()
    {
        if (targetPlayer == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        FaceTarget();

        if (distance <= attackDistance)
        {
            agent.isStopped = true;

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
            ChaseDiagonal(distance);
        }

        Animate();
    }

    void ChaseDiagonal(float distance)
    {
        Vector3 dirToPlayer = (targetPlayer.position - transform.position).normalized;
        dirToPlayer.y = 0f;

        Vector3 sideDir = Vector3.Cross(Vector3.up, dirToPlayer).normalized;

        float sideWave = Mathf.Sin(Time.time * strafeSpeed) * diagonalOffset;
        Vector3 diagonalTarget = targetPlayer.position
                                 - dirToPlayer * attackDistance
                                 + sideDir * sideWave * strafeSide;

        agent.SetDestination(diagonalTarget);
    }

    void FaceTarget()
    {
        Vector3 dir = targetPlayer.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            8f * Time.deltaTime
        );
    }

    void Attack()
    {
        if (animator != null && HasParameter("Attack"))
            animator.SetTrigger("Attack");

        PlayerHealth hp = targetPlayer.GetComponent<PlayerHealth>();

        if (hp != null)
            hp.TakeDamage(damage);

        Debug.Log(gameObject.name + " attacked " + targetPlayer.name);
    }

    public void TakeHit(GameObject attacker)
    {
        hitsTaken++;

        if (animator != null && HasParameter("Hit"))
            animator.SetTrigger("Hit");

        Debug.Log(gameObject.name + " hit " + hitsTaken + "/" + hitsToDie);

        if (hitsTaken >= hitsToDie)
            Die(attacker);
    }

    void Die(GameObject killerPlayer)
    {
        if (animator != null && HasParameter("Die"))
            animator.SetTrigger("Die");

        if (agent != null)
            agent.isStopped = true;

        if (BossFightManager.Instance != null)
            BossFightManager.Instance.BossKilled(this, killerPlayer);

        enabled = false;
    }

    void Animate()
    {
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;

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
}