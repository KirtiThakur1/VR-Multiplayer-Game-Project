using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class Steering : MonoBehaviour
{
    [Header("XR")]
    public XROrigin xrOrigin;

    [Header("Ground Following")]
    public LayerMask groundLayer;
    public float rayStartHeight = 2.5f;
    public float rayLength = 8f;

    [Header("Y Movement")]
    public float upSpeed = 25f;
    public float downSpeed = 10f;
    public float yDeadZone = 0.01f;

    CharacterController cc;

    void Awake()
    {
        if (xrOrigin == null)
            xrOrigin = GetComponent<XROrigin>();

        // CharacterController روی همون GameObjectی هست که اسکریپت روشه (Lobby-HMDUser)
        cc = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        ApplyGroundFollowing();
    }

    void ApplyGroundFollowing()
    {
        if (xrOrigin == null || xrOrigin.Camera == null)
            return;

        // اگر CC نداری، بهتره اصلاً تلاش نکنیم چون با لوکوموشن تداخل می‌خوره
        if (cc == null)
            return;

        // Ray از بالای موقعیت فعلی CC (مرجع حرکتی واقعی)
        Vector3 rayOrigin = cc.transform.position + Vector3.up * rayStartHeight;

        Debug.DrawRay(rayOrigin, Vector3.down * rayLength, Color.yellow);

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength,
                groundLayer, QueryTriggerInteraction.Ignore))
            return;

        // در Floor mode، Y هدف همون سطح زمین هست
        bool isFloor = xrOrigin.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor;

        float targetY;
        if (isFloor)
        {
            targetY = hit.point.y;
        }
        else
        {
            float headHeight = xrOrigin.Camera.transform.position.y - cc.transform.position.y;
            targetY = hit.point.y - headHeight;
        }

        float currentY = cc.transform.position.y;
        float deltaY = targetY - currentY;

        if (Mathf.Abs(deltaY) < yDeadZone)
            return;

        float speed = (deltaY > 0f) ? upSpeed : downSpeed;

        // حرکت نرم در Y
        float moveY = Mathf.Lerp(0f, deltaY, Time.deltaTime * speed);

        // ✅ مهم: با CC.Move جابه‌جا کن تا برخورد و سیستم حرکت درست بماند
        cc.Move(new Vector3(0f, moveY, 0f));

        // Debug.Log($"GF hit={hit.collider.name} targetY={targetY} currY={currentY} dy={deltaY}");
    }
}
