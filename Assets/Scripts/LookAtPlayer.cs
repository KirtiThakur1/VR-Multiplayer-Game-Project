using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform head;

    private void Start()
    {
        head = Camera.main.transform;
    }

    private void Update()
    {
        if (head == null) return;

        Vector3 dir = head.position - transform.position;
        dir.y = 0;

        transform.rotation = Quaternion.LookRotation(dir);
    }
}