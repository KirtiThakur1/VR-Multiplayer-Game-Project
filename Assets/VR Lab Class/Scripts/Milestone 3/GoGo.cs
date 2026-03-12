using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace VRLabClass.Milestone3
{
    public class GoGo : MonoBehaviour
    {
        #region Properties

        [Header("Calculation Origin Configuration")]
        [SerializeField] private Transform _head; // Transform of user head --> used for origin calculation
        [SerializeField] private float _bodyCenterHeadOffset = .2f; // Vertical offset used to determine body center below users head

        private Vector3 _bodyCenter // returns position of body center used for calculation
        {
            get
            {
                Vector3 v = _head.position;
                v.y -= _bodyCenterHeadOffset;

                return v;
            }
        }

        [Header("GoGo Configuration")]
        [SerializeField] private Transform _hand; // Transform of users real hand
        [SerializeField] private Transform _gogoHand; // Hand transform to apply GoGo movement to
        [SerializeField] private GameObject _gogoVisual; // Hand visual that should be applied as soon as gogog hand exceeds the 1:1 mapping distance threshold
        [SerializeField][Range(0, 1)] private float _k = .167f; // value k in gogo equation
        [SerializeField][Range(0, 1)] private float _distanceThreshold = .4f; // value D in gogo equation

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            // Delete component if attached to remote users avatar
            if (GetComponentInParent<NetworkObject>() != null)
                if (!GetComponentInParent<NetworkObject>().IsOwner)
                {
                    Destroy(this);
                    return;
                }

            // set gogo hand to initial position and rotation, aligned with real hand
            _gogoHand.position = _hand.position;
            _gogoHand.rotation = _hand.rotation;

            // initially deactivate visuals
            _gogoVisual.SetActive(false);
        }

        private void Update()
        {
            ApplyGoGo();
        }

        #endregion

        #region GoGo Methods

        private void ApplyGoGo()
        {
            // Get real distance from body center to hand in meters
            float Rr = Vector3.Distance(_bodyCenter, _hand.position);

            // Convert to cm for GoGo formula (paper uses cm)
            float Rr_cm = Rr * 100f;
            float D_cm = _distanceThreshold * 100f;

            float Rv_cm;

            if (Rr_cm < D_cm)
            {
                // Within threshold: normal 1:1 mapping
                Rv_cm = Rr_cm;
                _gogoVisual.SetActive(false);
            }
            else
            {
                // Beyond threshold: non-linear GoGo extension
                Rv_cm = Rr_cm + _k * Mathf.Pow(Rr_cm - D_cm, 2f);
                _gogoVisual.SetActive(true);
            }

            // Convert back to meters
            float Rv = Rv_cm / 100f;

            // Move gogoHand along direction from bodyCenter to real hand
            Vector3 direction = (_hand.position - _bodyCenter).normalized;
            _gogoHand.position = _bodyCenter + direction * Rv;

            // Match rotation of real hand
            _gogoHand.rotation = _hand.rotation;
        }

        /*private void ApplyGoGo()
        {
            // HERE: Implementations for 3.3
            // Calculate offset according to GoGo equation
            // Apply offset to _gogoHand
            // Activate _gogVisuals if exceeding _distanceThreshold
        }*/

        #endregion
    }
}
