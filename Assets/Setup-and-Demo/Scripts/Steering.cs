using UnityEngine;
using Unity.XR.CoreUtils;

public class Steering : MonoBehaviour
{
    public XROrigin xrOrigin;

    [Header("Ground Following")]
    public LayerMask groundLayer;     
    public float rayStartHeight = 2.5f;
    public float rayLength = 8f;

    public float upSpeed = 25f;       
    public float downSpeed = 10f;      
    public float yDeadZone = 0.01f;    

    void Update()
    {
       

      
        ApplyGroundFollowing();
    }

    void ApplyGroundFollowing()
    {
        if (xrOrigin == null || xrOrigin.Camera == null) return;

        Vector3 rayOrigin = xrOrigin.transform.position + Vector3.up * rayStartHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength,
            groundLayer, QueryTriggerInteraction.Ignore))
        {
          
            float headHeight = xrOrigin.Camera.transform.position.y - xrOrigin.transform.position.y;

         
            float targetY = hit.point.y - headHeight;

            Vector3 p = xrOrigin.transform.position;

           
            if (Mathf.Abs(targetY - p.y) < yDeadZone) return;

            float speed = (targetY > p.y) ? upSpeed : downSpeed;
            p.y = Mathf.Lerp(p.y, targetY, Time.deltaTime * speed);

            xrOrigin.transform.position = p;
        }

    }
}
