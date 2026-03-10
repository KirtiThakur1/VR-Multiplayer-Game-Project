using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ToggleGrab : MonoBehaviour
{
    [Header("References")]
    public XRGrabInteractable grab;
    public XRDirectInteractor rightHand;

    [Header("Auto-Find")]
    public string rightHandDirectObjectName = "RightHand Direct";

    [Header("Input")]
    public InputActionProperty gripAction;

    private bool isHeld;
    private bool inputLocked;

    void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        if (rightHand == null)
        {
            var go = GameObject.Find(rightHandDirectObjectName);
            if (go != null)
                rightHand = go.GetComponent<XRDirectInteractor>();
        }

        if (rightHand == null)
        {
            Debug.LogError($"[ToggleGrab] Could not find XRDirectInteractor on '{rightHandDirectObjectName}'.");
        }
    }

    void OnEnable()
    {
        if (gripAction.action == null)
        {
            Debug.LogWarning("[ToggleGrab] Grip Action is not assigned.");
            return;
        }

        gripAction.action.Enable();
        gripAction.action.performed += OnGripPerformed;
        gripAction.action.canceled += OnGripReleased;
    }

    void OnDisable()
    {
        if (gripAction.action == null) return;

        gripAction.action.performed -= OnGripPerformed;
        gripAction.action.canceled -= OnGripReleased;
        gripAction.action.Disable();
    }

    void OnGripPerformed(InputAction.CallbackContext ctx)
    {
        if (inputLocked) return;
        inputLocked = true;

        if (grab == null || rightHand == null) return;

        var manager = grab.interactionManager;
        if (manager == null)
        {
            Debug.LogWarning("[ToggleGrab] Interaction Manager is null.");
            return;
        }

        IXRSelectInteractor interactor = rightHand;
        IXRSelectInteractable interactable = grab;

        if (!isHeld)
        {
            manager.SelectEnter(interactor, interactable);
            isHeld = true;
            Debug.Log("[ToggleGrab] Grabbed");
        }
        else
        {
            manager.SelectExit(interactor, interactable);
            isHeld = false;
            Debug.Log("[ToggleGrab] Released");
        }
    }

    void OnGripReleased(InputAction.CallbackContext ctx)
    {
        inputLocked = false;
    }
}