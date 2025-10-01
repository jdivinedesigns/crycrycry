using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;

    public Vector3 localOffset;

    public Vector3 localEulerOffset;

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.TransformPoint(localOffset);
        transform.rotation = target.rotation * Quaternion.Euler(localEulerOffset);
    }
}