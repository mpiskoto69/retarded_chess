using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Follow")]
    public Vector3 offset = new Vector3(0f, 4f, -7f);
    public float followSpeed = 10f;
    public float turnSpeed = 10f;

    [Header("Look")]
    public float lookHeight = 1.5f;

    [Header("Look Back")]
    public KeyCode lookBackKey = KeyCode.Q;

    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = offset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 wantedOffset = offset;

        if (Input.GetKey(lookBackKey))
            wantedOffset = new Vector3(-offset.x, offset.y, -offset.z);

        currentOffset = Vector3.Lerp(
            currentOffset,
            wantedOffset,
            turnSpeed * Time.deltaTime
        );

        Vector3 desiredPosition = target.position + currentOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}