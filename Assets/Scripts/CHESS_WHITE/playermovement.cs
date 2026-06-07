using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.LeftShift;
    public KeyCode attackKey = KeyCode.LeftControl;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float turnSpeed = 10f;
    public float gravity = -25f;
    public float jumpHeight = 1.2f;

    [Header("Board Limits")]
    public bool useBoardLimits = true;
    public float minX = 14212.71f;
    public float maxX = 15179.92f;
    public float minZ = 7216.017f;
    public float maxZ = 8179.771f;

    [Header("Model Fix")]
    public float modelYawOffset = 0f;

    [Header("Animator")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float yVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    void Update()
    {
        ReadMovement();
        ApplyMovement();
        HandleJump();
        HandleAttack();
        HandleAnimations();
    }

    void ReadMovement()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(forwardKey)) z += 1f;
        if (Input.GetKey(backwardKey)) z -= 1f;
        if (Input.GetKey(rightKey)) x += 1f;
        if (Input.GetKey(leftKey)) x -= 1f;

        moveDirection = new Vector3(x, 0f, z);

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();
    }

    void ApplyMovement()
    {
        if (controller.isGrounded && yVelocity < 0f)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (useBoardLimits)
            ClampToBoard();

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection) *
                Quaternion.Euler(0f, modelYawOffset, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }

    void ClampToBoard()
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

    void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) && controller.isGrounded)
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null && HasParameter("Jump"))
                animator.SetTrigger("Jump");
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(attackKey))
        {
            if (animator != null && HasParameter("Attack"))
                animator.SetTrigger("Attack");
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        if (HasParameter("IsMoving"))
            animator.SetBool("IsMoving", isMoving);

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", isMoving ? 1f : 0f);

        if (HasParameter("IsGrounded"))
            animator.SetBool("IsGrounded", controller.isGrounded);
    }

    bool HasParameter(string name)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == name)
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