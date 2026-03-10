using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AxeOwnership : MonoBehaviour
{
    public AxeSlicer axeSlicer;
    public XRGrabInteractable grabInteractable;

    private void Awake()
    {
        if (axeSlicer == null)
            axeSlicer = GetComponent<AxeSlicer>();

        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (axeSlicer == null)
            return;

        Transform interactorTransform = args.interactorObject.transform;
        if (interactorTransform == null)
            return;

        PlayerScore playerScore = interactorTransform.GetComponentInParent<PlayerScore>();

        if (playerScore == null)
        {
            Debug.LogWarning("Axe grabbed, but no PlayerScore found on interactor parent.");
            return;
        }

        axeSlicer.ownerScore = playerScore;
        Debug.Log($"Axe owner set to {playerScore.name}");
    }
}