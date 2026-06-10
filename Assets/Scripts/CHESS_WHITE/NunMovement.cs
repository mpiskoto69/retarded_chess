using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NunMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -25f;

    [Header("Input Fix")]
    public bool invertForward = false;
    public bool invertSide = false;

    [Header("Board Limits")]
    public bool useBoardLimits = true;
    public float minX = 421f;
    public float maxX = 581f;
    public float minZ = 549f;
    public float maxZ = 709f;

    [Header("Spawn Safety")]
    public bool clampOnStart = true;
    public float startY = 1f;
    public bool forceStartY = false;

    [Header("Model Facing Fix")]
    public float modelYawOffset = 0f;

    [Header("Animator")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 moveDir;
    private float verticalVelocity;
    private Quaternion targetRotation;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        targetRotation = transform.rotation;
    }

    void Start()
    {
        if (forceStartY)
        {
            Vector3 p = transform.position;
            p.y = startY;
            transform.position = p;
        }

        if (clampOnStart && useBoardLimits)
            ClampTransformToBoard();

        targetRotation = transform.rotation;
    }

    void Update()
    {
        ReadInput();
        Move();
        Jump();
        Attack();
        Animate();
    }

    void ReadInput()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) z += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) z -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) x += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) x -= 1f;

        if (invertForward)
            z *= -1f;

        if (invertSide)
            x *= -1f;

        moveDir = new Vector3(x, 0f, z);

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        if (moveDir.sqrMagnitude > 0.01f)
        {
            targetRotation =
                Quaternion.LookRotation(moveDir) *
                Quaternion.Euler(0f, modelYawOffset, 0f);
        }
    }

    void Move()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        Vector3 motion = moveDir * moveSpeed;
        motion.y = verticalVelocity;

        controller.Move(motion * Time.deltaTime);

        if (useBoardLimits)
            ClampControllerToBoard();
    }

    void ClampControllerToBoard()
    {
        Vector3 p = transform.position;

        float clampedX = Mathf.Clamp(p.x, minX, maxX);
        float clampedZ = Mathf.Clamp(p.z, minZ, maxZ);

        Vector3 correction = new Vector3(
            clampedX - p.x,
            0f,
            clampedZ - p.z
        );

        if (correction.sqrMagnitude > 0.0001f)
            controller.Move(correction);
    }

    void ClampTransformToBoard()
    {
        Vector3 p = transform.position;

        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.z = Mathf.Clamp(p.z, minZ, maxZ);

        transform.position = p;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null && HasParameter("Jump"))
                animator.SetTrigger("Jump");
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (animator != null && HasParameter("Attack"))
                animator.SetTrigger("Attack");
        }
    }

    void Animate()
    {
        if (animator == null) return;

        bool moving = moveDir.sqrMagnitude > 0.01f;

        if (HasParameter("IsMoving"))
            animator.SetBool("IsMoving", moving);

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", moving ? 1f : 0f);

        if (HasParameter("IsGrounded"))
            animator.SetBool("IsGrounded", controller.isGrounded);
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

    void OnDrawGizmosSelected()
    {
        if (!useBoardLimits) return;

        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (minX + maxX) * 0.5f,
            transform.position.y + 0.05f,
            (minZ + maxZ) * 0.5f
        );

        Vector3 size = new Vector3(
            maxX - minX,
            0.1f,
            maxZ - minZ
        );

        Gizmos.DrawWireCube(center, size);
    }
}