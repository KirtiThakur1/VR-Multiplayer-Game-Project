using Unity.Netcode;
using UnityEngine;

namespace VRLabClass.Milestone3
{
    //public class GrabPolicy : MonoBehaviour
    public class GrabPolicy : NetworkBehaviour
    {
        #region Properties

        private NetworkVariable<bool> _isGrabbed = new NetworkVariable<bool>(false,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        #endregion

        #region Policy Methods

        public bool RequestAccess()
        {
            // Topic 3.4 implementation
            if (_isGrabbed.Value)
                return false;

            // Grant access - transfer ownership via RPC
            RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
            return true;
            // HERE: Implementations for 3.4
            // if _isGrabbed --> return false
            // if !_isGrabbed --> give ownership to local user, set _isGrabbed to true, return true


        }

        public void Release()
        {
            // HERE: Implementations for 3.4
            ReleaseServerRpc();
            // update _isGrabbed to false
        }

        #endregion

        #region RPCs

        // HERE: Implementations for 3.4
        // implement a RPC to update _isGrabbed
        // implement a RPC to change ownership
        [ServerRpc(RequireOwnership = false)]
        private void RequestOwnershipServerRpc(ulong clientId)
        {
            NetworkObject.ChangeOwnership(clientId);
            _isGrabbed.Value = true;
            Debug.Log($"[GrabPolicy] Ownership given to client {clientId}");
        }

        [ServerRpc(RequireOwnership = false)]
        private void ReleaseServerRpc()
        {
            _isGrabbed.Value = false;
            Debug.Log("[GrabPolicy] Object released");
        }

        #endregion
    }
}
