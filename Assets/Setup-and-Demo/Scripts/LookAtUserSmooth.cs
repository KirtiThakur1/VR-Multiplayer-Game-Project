using UnityEngine;

public class LookAtUserSmooth : MonoBehaviour
{
    public Transform userHead;        // XR Main Camera / HMD
    public float rotateSpeed = 8f;
    public bool yawOnly = true;

    void LateUpdate()
    {
        if (!userHead) return;

        Vector3 dir = userHead.position - transform.position;
        if (yawOnly) dir.y = 0f;

        if (dir.sqrMagnitude < 0.00001f) return;

        Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotateSpeed * Time.deltaTime);
    }
}