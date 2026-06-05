using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform target;

    public float distance = 7f;
    public float height = 4f;
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 wantedPosition =
            target.position
            - target.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            wantedPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}