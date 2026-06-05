using UnityEngine;

public class NunMovementBoss : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 12f;
    public float jumpHeight = 1.2f;
    public float gravity = -25f;

    [Header("Camera")]
    public Transform cameraTransform;

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

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
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

        Vector3 input = new Vector3(x, 0f, z).normalized;

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            moveDir = camForward * input.z + camRight * input.x;
        }
        else
        {
            moveDir = input;
        }

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        IsMoving = moveDir.sqrMagnitude > 0.01f;
    }

    void Move()
    {
        IsGrounded = controller.isGrounded;

        if (IsGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 motion = moveDir * moveSpeed;
        motion.y = verticalVelocity;

        controller.Move(motion * Time.deltaTime);

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

            if (animator != null && HasParameter("Jump"))
                animator.SetTrigger("Jump");
        }
    }

    void HandleAttack()
    {
        AttackPressed = Input.GetKeyDown(KeyCode.Return);

        if (AttackPressed && animator != null && HasParameter("Attack"))
            animator.SetTrigger("Attack");
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        if (HasParameter("IsMoving"))
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