using UnityEngine;
using Unity.Netcode;

public class RoleAssigner : MonoBehaviour
{
    public NetworkObject HMDPrefab;       // Lobby-HMDUser
    public NetworkObject DesktopPrefab;   // DesktopUserPrefab

    void Awake()
    {
        bool isClone = PlayerPrefs.GetInt("IsClone", 0) == 1;

        // Assign the GameObject, not the NetworkObject component
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab =
            (isClone ? DesktopPrefab : HMDPrefab).gameObject;
    }
}
