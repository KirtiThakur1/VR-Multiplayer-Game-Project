using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapEnabler : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionAsset inputActions;

    [Header("Enable These")]
    public string actionMapToEnable;

    [Header("Disable These")]
    public string actionMapToDisable;

    void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset not assigned!", this);
            return;
        }

        inputActions.FindActionMap(actionMapToDisable)?.Disable();
        inputActions.FindActionMap(actionMapToEnable)?.Enable();
    }
}

