using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GrimReaperBoss : MonoBehaviour
{
    public Transform player;
    public Transform axe;

    public int hitsToDie = 3;
    public float damage = 0.5f;

    public float attackDistance = 3.5f;
    public float attackCooldown = 1.5f;

    public float teleportCooldown = 5f;
    public Transform[] teleportPoints;

    public string returnSceneName = "first";

    private int hitsTaken = 0;
    private float nextAttackTime;
    private float nextTeleportTime;

    private NavMeshAgent agent;
    private Vector3 axeStartLocalRot;
    private bool attacking = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (axe != null)
            axeStartLocalRot = axe.localEulerAngles;
    }

    void Update()
    {
        if (player == null) return;

        ChasePlayer();
        DragMotion();

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
            nextAttackTime = Time.time + attackCooldown;
        }

        if (Time.time >= nextTeleportTime)
        {
            TeleportDiagonal();
            nextTeleportTime = Time.time + teleportCooldown;
        }
    }

    void ChasePlayer()
    {
        if (agent == null) return;

        agent.SetDestination(player.position);
    }

    void DragMotion()
    {
        float sway = Mathf.Sin(Time.time * 4f) * 3f;
        transform.localRotation = Quaternion.Euler(0f, transform.eulerAngles.y, sway);
    }

    System.Collections.IEnumerator Attack()
    {
        if (attacking) yield break;
        attacking = true;

        if (agent != null)
            agent.isStopped = true;

        float t = 0f;

        while (t < 0.15f)
        {
            t += Time.deltaTime;

            if (axe != null)
                axe.localEulerAngles = axeStartLocalRot + new Vector3(-80f * (t / 0.15f), 0f, 0f);

            yield return null;
        }

        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();

            if (hp != null)
                hp.TakeDamage(damage);
        }

        t = 0f;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            if (axe != null)
                axe.localEulerAngles = axeStartLocalRot;

            yield return null;
        }

        if (agent != null)
            agent.isStopped = false;

        attacking = false;
    }

    public void TakeHit()
    {
        hitsTaken++;

        if (hitsTaken >= hitsToDie)
            Die();
    }

    void TeleportDiagonal()
    {
        if (teleportPoints == null || teleportPoints.Length == 0) return;

        Transform p = teleportPoints[Random.Range(0, teleportPoints.Length)];

        if (agent != null)
            agent.Warp(p.position);
        else
            transform.position = p.position;
    }

    void Die()
    {
        SceneManager.LoadScene(returnSceneName);
    }
}