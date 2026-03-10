using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ReturnToStartOnRelease : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;

    private XRGrabInteractable grab;
    private Rigidbody rb;

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grab.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        grab.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        transform.SetPositionAndRotation(startPos, startRot);
        rb.isKinematic = false;
    }
}
