using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = -90f;   // rotation when opened
    public float closedAngle = 0f;   // rotation when closed
    public float duration = 1f;      // how fast

    private bool isOpen = false;
    private Quaternion openRot;
    private Quaternion closedRot;
    private float t = 0f;

    void Start()
    {
        closedRot = Quaternion.Euler(0f, closedAngle, 0f);
        openRot = Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        // smooth lerp between open/close
        Quaternion target = isOpen ? openRot : closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * (1f / duration));
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}

