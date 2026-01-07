using UnityEngine;
using UnityEngine.InputSystem;

public class HMDInputOverride : MonoBehaviour
{
    [Header("Shared Input Actions Asset")]
    public InputActionAsset inputActions;

    void Start()
    {
        if (inputActions == null)
        {
            Debug.LogError("HMDInputOverride: InputActionAsset not assigned!", this);
            return;
        }

        // XR Toolkit just enabled everything — now we correct it
        inputActions.FindActionMap("Player_Desktop")?.Disable();

        // Optional hard stop
        if (Keyboard.current != null)
            InputSystem.DisableDevice(Keyboard.current);

        if (Mouse.current != null)
            InputSystem.DisableDevice(Mouse.current);
    }
}
