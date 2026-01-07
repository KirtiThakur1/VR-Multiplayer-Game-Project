#if UNITY_EDITOR
using ParrelSync;
#endif
using UnityEngine;
using VRSYS.Core.Networking;

public class RoleSelector : MonoBehaviour
{
    void Awake()
    {
#if UNITY_EDITOR
        bool isClone = ClonesManager.IsClone();
#else
        bool isClone = false;
#endif
        if (isClone)
            ConnectionManager.Instance.userSpawnInfo.userRole = new UserRole("Desktop");
        else
            ConnectionManager.Instance.userSpawnInfo.userRole = new UserRole("HMD");
    }
}
