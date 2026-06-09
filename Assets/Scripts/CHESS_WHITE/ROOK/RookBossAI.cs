using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RookBossAI : MonoBehaviour
{
    public enum RookMoveMode
    {
        ChaseOnly,
        ChargeWhenAligned,
        ChargeOnCooldown
    }

    public Transform targetPlayer;

    [Header("Health")]
    public int hitsToDie = 3;

    [Header("Damage")]
    public float damage = 0.5f;
    public float hitRange = 1.4f;
    public float hitCooldown = 1f;
    public bool damageOnTouch = true;

    [Header("Hit Rules")]
    public bool canOnlyBeHitWhileStunned = true;

    [Header("Movement")]
    public RookMoveMode moveMode = RookMoveMode.ChargeWhenAligned;
    public float moveSpeed = 2.5f;
    public float stoppingDistance = 2f;
    public float turnSpeed = 8f;
    public float modelYawOffset = 0f;

    [Header("Model Visual Adjust")]
    public Transform modelRoot;
    public Vector3 modelLocalPosition = Vector3.zero;
    public Vector3 modelLocalRotation = Vector3.zero;
    public Vector3 modelLocalScale = Vector3.one;

    [Header("Rook Charge")]
    public float alignDistance = 1.5f;
    public float chargeSpeed = 12f;
    public float chargeDistance = 12f;
    public float chargeWindup = 0.7f;
    public float chargeCooldown = 3f;
    public float stunTime = 1.5f;

    [Header("Death")]
    public float disappearDelay = 1.5f;

    [Header("Animator")]
    public Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chargeSound;
    public AudioClip impactSound;
    public AudioClip deathSound;

    private NavMeshAgent agent;
    private int hitsTaken = 0;
    private float nextHitTime = 0f;
    private float nextChargeTime = 0f;
    private bool isDead = false;
    private bool deathHandled = false;
    private bool isCharging = false;
    private bool isStunned = false;

    public int CurrentHits => hitsTaken;
    public int MaxHits => hitsToDie;
    public bool IsStunned => isStunned;

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

        ApplyModelVisualAdjust();
    }

    void Update()
    {
        if (isDead || targetPlayer == null) return;

        if (isCharging || isStunned)
        {
            UpdateAnimator();
            return;
        }

        FaceTarget();

        if (ShouldStartCharge())
        {
            StartCoroutine(ChargeRoutine());
            nextChargeTime = Time.time + chargeCooldown;
            return;
        }

        ChasePlayer();

        if (damageOnTouch)
            TouchDamage();

        UpdateAnimator();
    }

    bool ShouldStartCharge()
    {
        if (Time.time < nextChargeTime)
            return false;

        if (moveMode == RookMoveMode.ChaseOnly)
            return false;

        if (moveMode == RookMoveMode.ChargeOnCooldown)
            return true;

        return IsAlignedWithPlayer();
    }

    bool IsAlignedWithPlayer()
    {
        Vector3 diff = targetPlayer.position - transform.position;
        diff.y = 0f;

        bool alignedX = Mathf.Abs(diff.x) <= alignDistance;
        bool alignedZ = Mathf.Abs(diff.z) <= alignDistance;

        return alignedX || alignedZ;
    }

    void ChasePlayer()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.isStopped = false;
        agent.SetDestination(targetPlayer.position);
    }

    IEnumerator ChargeRoutine()
    {
        isCharging = true;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (animator != null && HasParameter("Charge"))
            animator.SetTrigger("Charge");

        yield return new WaitForSeconds(chargeWindup);

        Vector3 chargeDir = GetRookChargeDirection();

        if (chargeDir.sqrMagnitude < 0.01f)
            chargeDir = transform.forward;

        transform.rotation =
            Quaternion.LookRotation(chargeDir) *
            Quaternion.Euler(0f, modelYawOffset, 0f);

        if (audioSource != null && chargeSound != null)
            audioSource.PlayOneShot(chargeSound);

        float traveled = 0f;

        while (traveled < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime;

            transform.position += chargeDir * step;
            traveled += step;

            if (damageOnTouch)
                TouchDamage();

            yield return null;
        }

        if (audioSource != null && impactSound != null)
            audioSource.PlayOneShot(impactSound);

        isCharging = false;
        StartCoroutine(StunRoutine());
    }

    Vector3 GetRookChargeDirection()
    {
        Vector3 diff = targetPlayer.position - transform.position;
        diff.y = 0f;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
            return new Vector3(Mathf.Sign(diff.x), 0f, 0f);

        return new Vector3(0f, 0f, Mathf.Sign(diff.z));
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;

        if (animator != null && HasParameter("Stunned"))
            animator.SetBool("Stunned", true);

        yield return new WaitForSeconds(stunTime);

        if (animator != null && HasParameter("Stunned"))
            animator.SetBool("Stunned", false);

        isStunned = false;
    }

    void TouchDamage()
    {
        if (targetPlayer == null) return;

        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(targetPlayer.position.x, targetPlayer.position.z)
        );

        if (distance > hitRange)
            return;

        DamagePlayer();
    }

    void DamagePlayer()
    {
        if (Time.time < nextHitTime) return;

        nextHitTime = Time.time + hitCooldown;

        PlayerHealth hp = targetPlayer.GetComponent<PlayerHealth>();

        if (hp == null)
            hp = targetPlayer.GetComponentInParent<PlayerHealth>();

        if (hp == null)
            hp = targetPlayer.GetComponentInChildren<PlayerHealth>();

        if (hp != null)
        {
            Debug.Log(name + " damages " + hp.name);
            hp.TakeDamage(damage);
        }
        else
        {
            Debug.LogError("Target player has no PlayerHealth: " + targetPlayer.name);
        }
    }

    public void TakeHit(GameObject attacker)
    {
        if (isDead) return;

        if (canOnlyBeHitWhileStunned && !isStunned)
        {
            Debug.Log(name + " can only be hit while stunned!");
            return;
        }

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

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

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

        if (BossRookManager.Instance != null)
            BossRookManager.Instance.BossKilled(this, killerPlayer);
        else
            Debug.LogError("No BossRookManager Instance found!");

        Destroy(gameObject, disappearDelay);
    }

    void FaceTarget()
    {
        Vector3 dir = targetPlayer.position - transform.position;
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

    void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = 0f;

        if (agent != null && agent.enabled)
            speed = agent.velocity.magnitude;

        if (isCharging)
            speed = chargeSpeed;

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", speed);

        if (HasParameter("IsMoving"))
            animator.SetBool("IsMoving", speed > 0.1f && !isStunned);
    }

    void ApplyModelVisualAdjust()
    {
        if (modelRoot == null) return;

        modelRoot.localPosition = modelLocalPosition;
        modelRoot.localRotation = Quaternion.Euler(modelLocalRotation);
        modelRoot.localScale = modelLocalScale;
    }

    void OnValidate()
    {
        ApplyModelVisualAdjust();
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
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}