using UnityEngine;

public class NunCodeAnimation : MonoBehaviour
{
    [Header("Leg Bones")]
    public Transform rightHip;
    public Transform leftHip;
    public Transform rightKnee;
    public Transform leftKnee;

    [Header("Upper Body")]
    public Transform spine;
    public Transform chest;

    public Transform rightArm;
    public Transform leftArm;
    public Transform rightForearm;
    public Transform leftForearm;
    public Transform rightWrist;
    public Transform leftWrist;

    [Header("Lightsaber")]
    public GameObject saberHitbox;

    [Header("Run")]
    public float runSpeed = 11f;
    public float hipSwing = 14f;
    public float kneeBend = 18f;
    public float bodyLean = 4f;
    public float bodySway = 2f;

    [Header("Jump")]
    public float jumpPoseStrength = 0.55f;

    [Header("Attack")]
    public float attackDuration = 0.28f;
    public float attackLift = 75f;
    public float attackSlash = 85f;

    private Quaternion rHip0, lHip0, rKnee0, lKnee0;
    private Quaternion spine0, chest0;
    private Quaternion rArm0, lArm0, rFore0, lFore0, rWrist0, lWrist0;

    private bool attacking;
    private float attackTimer;

    void Awake()
    {
        rHip0 = Save(rightHip);
        lHip0 = Save(leftHip);
        rKnee0 = Save(rightKnee);
        lKnee0 = Save(leftKnee);

        spine0 = Save(spine);
        chest0 = Save(chest);

        rArm0 = Save(rightArm);
        lArm0 = Save(leftArm);
        rFore0 = Save(rightForearm);
        lFore0 = Save(leftForearm);
        rWrist0 = Save(rightWrist);
        lWrist0 = Save(leftWrist);

        if (saberHitbox != null)
            saberHitbox.SetActive(false);
    }

    Quaternion Save(Transform bone)
    {
        return bone != null ? bone.localRotation : Quaternion.identity;
    }

    void LateUpdate()
    {
        if (NunMovement.AttackPressed && !attacking)
            StartAttack();

        if (attacking)
            AnimateAttack();
        else if (!NunMovement.IsGrounded)
            AnimateJump();
        else if (NunMovement.IsMoving)
            AnimateRun();
        else
            AnimateIdle();
    }

    void AnimateRun()
    {
        float t = Time.time * runSpeed;

        float rightStep = Mathf.Sin(t);
        float leftStep = -rightStep;

        Set(rightHip, rHip0, -rightStep * hipSwing, 0f, 0f);
        Set(leftHip, lHip0, -leftStep * hipSwing, 0f, 0f);

        Set(rightKnee, rKnee0, Mathf.Max(0f, rightStep) * kneeBend, 0f, 0f);
        Set(leftKnee, lKnee0, Mathf.Max(0f, leftStep) * kneeBend, 0f, 0f);

        Set(spine, spine0, bodyLean, 0f, Mathf.Sin(t) * bodySway);
        Set(chest, chest0, 2f, 0f, Mathf.Sin(t) * bodySway * 0.5f);

        // combat run pose: χέρια μπροστά, σταθερά στο φωτόσπαθο
        Set(rightArm, rArm0, -32f, 0f, -18f);
        Set(rightForearm, rFore0, -48f, 0f, 0f);
        Set(rightWrist, rWrist0, -8f, 0f, 0f);

        Set(leftArm, lArm0, -30f, 0f, 22f);
        Set(leftForearm, lFore0, -50f, 0f, 0f);
        Set(leftWrist, lWrist0, -8f, 0f, 0f);
    }

    void AnimateJump()
    {
        float s = jumpPoseStrength;

        Set(spine, spine0, -8f * s, 0f, 0f);
        Set(chest, chest0, -4f * s, 0f, 0f);

        Set(rightHip, rHip0, -18f * s, 0f, 0f);
        Set(leftHip, lHip0, -18f * s, 0f, 0f);

        Set(rightKnee, rKnee0, 28f * s, 0f, 0f);
        Set(leftKnee, lKnee0, 28f * s, 0f, 0f);

        Set(rightArm, rArm0, -28f, 0f, -18f);
        Set(leftArm, lArm0, -28f, 0f, 18f);
    }

    void StartAttack()
    {
        attacking = true;
        attackTimer = attackDuration;

        if (saberHitbox != null)
            saberHitbox.SetActive(true);
    }

    void AnimateAttack()
    {
        float p = 1f - attackTimer / attackDuration;

        // γρήγορο σήκωμα και καθαρό χτύπημα μπροστά
        float lift = Mathf.Sin(Mathf.Clamp01(p) * Mathf.PI);
        float cut = Mathf.SmoothStep(0f, 1f, p);

        Set(spine, spine0, -6f * lift, 22f * cut, 0f);
        Set(chest, chest0, -4f * lift, 15f * cut, 0f);

        Set(rightArm, rArm0, -attackLift * lift, 0f, -24f);
        Set(rightForearm, rFore0, -attackSlash * cut, 0f, 0f);
        Set(rightWrist, rWrist0, -35f * cut, 0f, 0f);

        Set(leftArm, lArm0, -attackLift * 0.8f * lift, 0f, 24f);
        Set(leftForearm, lFore0, -attackSlash * 0.65f * cut, 0f, 0f);
        Set(leftWrist, lWrist0, -25f * cut, 0f, 0f);

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            attacking = false;

            if (saberHitbox != null)
                saberHitbox.SetActive(false);
        }
    }

    void AnimateIdle()
    {
        float breathe = Mathf.Sin(Time.time * 2f) * 1.2f;

        Set(spine, spine0, breathe, 0f, 0f);
        Set(chest, chest0, breathe * 0.5f, 0f, 0f);

        Set(rightHip, rHip0, 0f, 0f, 0f);
        Set(leftHip, lHip0, 0f, 0f, 0f);
        Set(rightKnee, rKnee0, 0f, 0f, 0f);
        Set(leftKnee, lKnee0, 0f, 0f, 0f);

        Set(rightArm, rArm0, -22f, 0f, -18f);
        Set(rightForearm, rFore0, -42f, 0f, 0f);
        Set(rightWrist, rWrist0, -5f, 0f, 0f);

        Set(leftArm, lArm0, -22f, 0f, 20f);
        Set(leftForearm, lFore0, -44f, 0f, 0f);
        Set(leftWrist, lWrist0, -5f, 0f, 0f);
    }

    void Set(Transform bone, Quaternion start, float x, float y, float z)
    {
        if (bone == null) return;

        Quaternion target = start * Quaternion.Euler(x, y, z);

        bone.localRotation = Quaternion.Slerp(
            bone.localRotation,
            target,
            Time.deltaTime * 16f
        );
    }
}