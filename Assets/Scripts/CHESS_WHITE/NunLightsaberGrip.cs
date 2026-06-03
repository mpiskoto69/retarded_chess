using UnityEngine;

public class NunLightsaberGrip : MonoBehaviour
{
    [Header("Lightsaber")]
    public Transform lightsaber;
    public Transform rightWrist;

    [Header("Arms")]
    public Transform rightUpperArm;
    public Transform rightForearm;
    public Transform leftUpperArm;
    public Transform leftForearm;
    public Transform leftWrist;

    [Header("Right Fingers")]
    public Transform rightThumb;
    public Transform rightIndex;
    public Transform rightMiddle;
    public Transform rightRing;
    public Transform rightPinky;

    [Header("Lightsaber Local Transform")]
    public Vector3 saberLocalPosition = new Vector3(0f, 0f, 0f);
    public Vector3 saberLocalRotation = new Vector3(0f, 90f, 0f);
    public Vector3 saberLocalScale = new Vector3(1f, 1f, 1f);

    [Header("Grip Pose")]
    public Vector3 rightArmRotation = new Vector3(-25f, 0f, -20f);
    public Vector3 rightForearmRotation = new Vector3(-45f, 0f, 0f);

    public Vector3 leftArmRotation = new Vector3(-25f, 0f, 25f);
    public Vector3 leftForearmRotation = new Vector3(-45f, 0f, 0f);
    public Vector3 leftWristRotation = new Vector3(0f, 0f, 15f);

    [Header("Finger Curl")]
    public Vector3 thumbCurl = new Vector3(0f, 0f, -35f);
    public Vector3 indexCurl = new Vector3(0f, 0f, 55f);
    public Vector3 middleCurl = new Vector3(0f, 0f, 60f);
    public Vector3 ringCurl = new Vector3(0f, 0f, 60f);
    public Vector3 pinkyCurl = new Vector3(0f, 0f, 60f);

    public float poseSpeed = 12f;

    private Quaternion rightArmStart;
    private Quaternion rightForearmStart;
    private Quaternion leftArmStart;
    private Quaternion leftForearmStart;
    private Quaternion leftWristStart;

    private Quaternion thumbStart;
    private Quaternion indexStart;
    private Quaternion middleStart;
    private Quaternion ringStart;
    private Quaternion pinkyStart;

    void Start()
    {
        SavePose();

        if (lightsaber != null && rightWrist != null)
        {
            lightsaber.SetParent(rightWrist);
            lightsaber.localPosition = saberLocalPosition;
            lightsaber.localRotation = Quaternion.Euler(saberLocalRotation);
            lightsaber.localScale = saberLocalScale;
        }
    }

    void LateUpdate()
    {
        HoldSaber();
        PoseArms();
        CurlFingers();
    }

    void SavePose()
    {
        if (rightUpperArm != null) rightArmStart = rightUpperArm.localRotation;
        if (rightForearm != null) rightForearmStart = rightForearm.localRotation;

        if (leftUpperArm != null) leftArmStart = leftUpperArm.localRotation;
        if (leftForearm != null) leftForearmStart = leftForearm.localRotation;
        if (leftWrist != null) leftWristStart = leftWrist.localRotation;

        if (rightThumb != null) thumbStart = rightThumb.localRotation;
        if (rightIndex != null) indexStart = rightIndex.localRotation;
        if (rightMiddle != null) middleStart = rightMiddle.localRotation;
        if (rightRing != null) ringStart = rightRing.localRotation;
        if (rightPinky != null) pinkyStart = rightPinky.localRotation;
    }

    void HoldSaber()
    {
        if (lightsaber == null || rightWrist == null)
            return;

        lightsaber.localPosition = saberLocalPosition;
        lightsaber.localRotation = Quaternion.Euler(saberLocalRotation);
        lightsaber.localScale = saberLocalScale;
    }

    void PoseArms()
    {
        float s = Time.deltaTime * poseSpeed;

        if (rightUpperArm != null)
            rightUpperArm.localRotation = Quaternion.Slerp(
                rightUpperArm.localRotation,
                rightArmStart * Quaternion.Euler(rightArmRotation),
                s
            );

        if (rightForearm != null)
            rightForearm.localRotation = Quaternion.Slerp(
                rightForearm.localRotation,
                rightForearmStart * Quaternion.Euler(rightForearmRotation),
                s
            );

        if (leftUpperArm != null)
            leftUpperArm.localRotation = Quaternion.Slerp(
                leftUpperArm.localRotation,
                leftArmStart * Quaternion.Euler(leftArmRotation),
                s
            );

        if (leftForearm != null)
            leftForearm.localRotation = Quaternion.Slerp(
                leftForearm.localRotation,
                leftForearmStart * Quaternion.Euler(leftForearmRotation),
                s
            );

        if (leftWrist != null)
            leftWrist.localRotation = Quaternion.Slerp(
                leftWrist.localRotation,
                leftWristStart * Quaternion.Euler(leftWristRotation),
                s
            );
    }

    void CurlFingers()
    {
        float s = Time.deltaTime * poseSpeed;

        if (rightThumb != null)
            rightThumb.localRotation = Quaternion.Slerp(
                rightThumb.localRotation,
                thumbStart * Quaternion.Euler(thumbCurl),
                s
            );

        if (rightIndex != null)
            rightIndex.localRotation = Quaternion.Slerp(
                rightIndex.localRotation,
                indexStart * Quaternion.Euler(indexCurl),
                s
            );

        if (rightMiddle != null)
            rightMiddle.localRotation = Quaternion.Slerp(
                rightMiddle.localRotation,
                middleStart * Quaternion.Euler(middleCurl),
                s
            );

        if (rightRing != null)
            rightRing.localRotation = Quaternion.Slerp(
                rightRing.localRotation,
                ringStart * Quaternion.Euler(ringCurl),
                s
            );

        if (rightPinky != null)
            rightPinky.localRotation = Quaternion.Slerp(
                rightPinky.localRotation,
                pinkyStart * Quaternion.Euler(pinkyCurl),
                s
            );
    }
}