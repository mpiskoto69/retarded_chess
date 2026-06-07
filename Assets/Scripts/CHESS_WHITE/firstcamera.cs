using UnityEngine;

public class firstcamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -12f);
    public float followSpeed = 8f;
    public float lookHeight = 2f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}