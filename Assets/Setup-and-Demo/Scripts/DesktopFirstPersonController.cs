// Scripts/DesktopFirstPersonController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopFirstPersonController : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;   // Desktop/Move
    public InputActionReference lookAction;   // Desktop/Look
    public InputActionReference sliceAction;  // Desktop/Slice

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 0.12f;
    public Transform cameraPivot; // assign the Camera transform

    float yaw, pitch;

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        sliceAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        sliceAction.action.Disable();
    }

    void Update()
    {
        // Move
        Vector2 m = moveAction.action.ReadValue<Vector2>();
        Vector3 wish = (transform.forward * m.y + transform.right * m.x) * moveSpeed;
        transform.position += wish * Time.deltaTime;

        // Look
        Vector2 look = lookAction.action.ReadValue<Vector2>();
        yaw += look.x * lookSensitivity;
        pitch -= look.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Slice (for desktop testing)
        if (sliceAction.action.triggered)
        {
            // Optional: raycast slice or play swing animation
        }
    }
}