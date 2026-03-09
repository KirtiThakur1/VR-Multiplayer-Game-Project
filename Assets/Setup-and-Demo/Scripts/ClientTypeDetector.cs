using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class ClientTypeDetector : MonoBehaviour
{
    public enum ClientType { VR_Host, Desktop_Client }
    public static ClientType CurrentType { get; private set; }
    
    void Awake()
    {
        #if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            CurrentType = ClientType.Desktop_Client;
            DisableXRForClone();
            Debug.Log("★★★ RUNNING AS DESKTOP CLIENT (Clone) ★★★");
        }
        else
        {
            CurrentType = ClientType.VR_Host;
            Debug.Log("★★★ RUNNING AS VR HOST (Original) ★★★");
        }
        #else
        // In builds, determine based on XR
        CurrentType = UnityEngine.XR.XRSettings.enabled 
            ? ClientType.VR_Host 
            : ClientType.Desktop_Client;
        #endif
    }
    
    void DisableXRForClone()
    {
        // Disable XR for desktop clone
        var xrManager = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager;
        if (xrManager != null)
        {
            xrManager.StopSubsystems();
            xrManager.DeinitializeLoader();
        }
        UnityEngine.XR.XRSettings.enabled = false;
        Debug.Log("XR Disabled for Desktop Clone");
    }
}