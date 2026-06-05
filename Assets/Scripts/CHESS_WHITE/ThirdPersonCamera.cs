using UnityEngine;
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -12f);
    public float followSpeed = 8f;
    public float lookHeight = 2f;

    void LateUpdate()
    {
        if (target == null) return;

        // Offset is FIXED in world space — camera never orbits
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}