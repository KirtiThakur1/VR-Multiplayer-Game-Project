using Unity.Netcode;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class NetworkAutoStarter : MonoBehaviour
{
    [SerializeField] private float clientStartDelay = 2f;

    void Start()
    {
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            // Clone = Desktop Client
            Debug.Log("Auto-starting as CLIENT in " + clientStartDelay + " seconds...");
            Invoke(nameof(StartClient), clientStartDelay);
        }
        else
        {
            // Original = VR Host
            Debug.Log("Auto-starting as HOST...");
            StartHost();
        }
#endif
    }

    void StartHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found!");
            return;
        }

        bool success = NetworkManager.Singleton.StartHost();
        if (success)
        {
            Debug.Log("✅ Started as HOST successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to start as HOST");
        }
    }

    void StartClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found!");
            return;
        }

        bool success = NetworkManager.Singleton.StartClient();
        if (success)
        {
            Debug.Log("✅ Started as CLIENT successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to start as CLIENT");
        }
    }
}