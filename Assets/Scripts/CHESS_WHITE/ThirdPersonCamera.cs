using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 7f;
    public float height = 4f;

    public float mouseSensitivity = 4f;

    float yaw;
    float pitch = 15f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target missing.");
            enabled = false;
            return;
        }

        yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -20f, 45f);

        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0);

        Vector3 offset =
            rotation * new Vector3(0, height, -distance);

        transform.position =
            target.position + offset;

        transform.LookAt(
            target.position + Vector3.up * 2f
        );
    }
}