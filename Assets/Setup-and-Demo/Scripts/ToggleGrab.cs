using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ToggleGrab : MonoBehaviour
{
    [Header("References")]
    public XRGrabInteractable grab;

    [Tooltip("Optional. If you can't assign in Inspector, leave empty and it will auto-find RightHand Direct.")]
    public XRDirectInteractor rightHand;

    [Header("Auto-Find (if Right Hand is not assigned)")]
    [Tooltip("Must match your Hierarchy object name. In your case: RightHand Direct")]
    public string rightHandDirectObjectName = "RightHand Direct";

    [Header("Input")]
    public InputActionProperty gripAction;

    private bool isHeld;

    void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        // Auto-find Right Hand Direct Interactor if not assigned
        if (rightHand == null)
        {
            var go = GameObject.Find(rightHandDirectObjectName);
            if (go != null)
                rightHand = go.GetComponent<XRDirectInteractor>();
        }

        if (rightHand == null)
            Debug.LogError($"[ToggleGrab] Could not find XRDirectInteractor on '{rightHandDirectObjectName}'. " +
                           $"Make sure that object exists and has an XR Direct Interactor component.");
    }

    void OnEnable()
    {
        if (gripAction.action == null)
        {
            Debug.LogWarning("[ToggleGrab] Grip Action is not assigned.");
            return;
        }

        gripAction.action.Enable();
        gripAction.action.performed += OnGrip;
    }

    void OnDisable()
    {
        if (gripAction.action == null) return;

        gripAction.action.performed -= OnGrip;
        gripAction.action.Disable();
    }

    void OnGrip(InputAction.CallbackContext ctx)
    {
        if (grab == null) return;
        if (rightHand == null) return;

        var manager = grab.interactionManager;
        if (manager == null)
        {
            Debug.LogWarning("[ToggleGrab] Interaction Manager is null. Assign XR Interaction Manager in scene.");
            return;
        }

        var interactor = (IXRSelectInteractor)rightHand;
        var interactable = (IXRSelectInteractable)grab;

        if (!isHeld)
        {
            manager.SelectEnter(interactor, interactable);
            isHeld = true;
        }
        else
        {
            manager.SelectExit(interactor, interactable);
            isHeld = false;
        }
    }
}
