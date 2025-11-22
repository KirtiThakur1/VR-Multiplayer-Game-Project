using UnityEngine;

public class ButterflyMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float turnSpeed = 2f;

    public float verticalAmplitude = 0.3f; 
    public float verticalSpeed = 2f;       

    
    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(5f, 2f, 5f);

    private Vector3 moveDir;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        float bob = Mathf.Sin(Time.time * verticalSpeed) * verticalAmplitude;

        Vector3 pos = transform.position;
        pos += moveDir * moveSpeed * Time.deltaTime;

        pos.y = areaCenter.y + bob;

        if (!IsInsideAreaXZ(pos))
        {
            PickNewDirection();
        }

        transform.position = pos;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }

        if (Random.value < 0.01f)
        {
            PickNewDirection();
        }
    }

    bool IsInsideAreaXZ(Vector3 pos)
    {
        Vector3 min = areaCenter - areaSize * 0.5f;
        Vector3 max = areaCenter + areaSize * 0.5f;

        return pos.x > min.x && pos.x < max.x &&
               pos.z > min.z && pos.z < max.z;
    }

    void PickNewDirection()
    {
        moveDir = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
