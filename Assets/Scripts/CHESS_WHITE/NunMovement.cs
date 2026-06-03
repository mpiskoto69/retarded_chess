using UnityEngine;

public class NunMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 18f;
    public float rotationSpeed = 18f;
    public float jumpHeight = 1.2f;
    public float gravity = -25f;

    [Header("Board Limits")]
    public float minX = 421f;
    public float maxX = 581f;
    public float minZ = 549f;
    public float maxZ = 709f;

    [Header("Animator")]
    public Animator animator;

    public static bool IsMoving;
    public static bool IsGrounded = true;
    public static bool AttackPressed;

    private CharacterController controller;
    private Vector3 moveDir;
    private float verticalVelocity;

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
        ReadInput();
        Move();
        HandleJump();
        HandleAttack();
        UpdateAnimator();
    }

    void ReadInput()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) z = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) z = -1f;

        moveDir = new Vector3(x, 0f, z).normalized;
        IsMoving = moveDir.sqrMagnitude > 0.01f;
    }

 void Move()
{
    if (controller == null) return;

    IsGrounded = controller.isGrounded;

    if (IsGrounded && verticalVelocity < 0f)
        verticalVelocity = -2f;

    verticalVelocity += gravity * Time.deltaTime;

    Vector3 horizontalMotion = moveDir * moveSpeed * Time.deltaTime;
    Vector3 verticalMotion = Vector3.up * verticalVelocity * Time.deltaTime;

    controller.Move(horizontalMotion);
    controller.Move(verticalMotion);

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

    if (IsMoving)
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    void HandleAttack()
    {
        AttackPressed = Input.GetKeyDown(KeyCode.Return);

        if (AttackPressed && animator != null)
            animator.SetTrigger("Attack");
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("IsMoving", IsMoving);

        if (HasParameter("IsGrounded"))
            animator.SetBool("IsGrounded", IsGrounded);

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", IsMoving ? 1f : 0f);
    }

    bool HasParameter(string parameterName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == parameterName)
                return true;
        }

        return false;
    }
}