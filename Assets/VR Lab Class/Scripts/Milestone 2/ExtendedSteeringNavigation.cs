using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VRLabClass.Milestone2
{
    public class ExtendedSteeringNavigation : MonoBehaviour
    {
        #region Enums

        // enum to select hand that determines forward direction and gives input
        private enum SteeringHandedness
        {
            Left,
            Right
        }

        #endregion

        #region Properties

        // Variables used for steering
        [Header("Steering Configuration")]
        [SerializeField] private SteeringHandedness _steeringHand = SteeringHandedness.Right;
        [SerializeField] private InputActionProperty _leftHandSteeringAction; // left trigger
        [SerializeField] private InputActionProperty _rightHandSteeringAction; // right trigger
        [SerializeField] private Transform _leftHandForwardIndicator; // transform is used to determine forward direction when steering with left hand
        [SerializeField] private Transform _rightHandForwardIndicator; // transform is used to determine forward direction when steering with right hand
        [SerializeField][Range(0f, 10f)] private float _maxSteeringSpeed = 3f; // FIX: changed from public to private
        [SerializeField] private bool _verticalSteering = false; // determines if y-axis is included in steering or not
        private float _currentSpeed = 0;

        [Header("Groundfollowing Configuration")]
        [SerializeField] private Transform _head;
        [SerializeField] private LayerMask _groundLayers; // selects which layers are seen as ground
        [SerializeField] private float _riseSpeed = 9.81f; // speed at which user is moved upwards, if position is too low
        private float _adjustmentThreshold = 0.01f; // distance to target height at which value is simply set
        private bool _isFalling = false;
        private float _fallStartTime;
        private bool _groundFound = false; // FIX: tracks whether raycast hit valid ground

        [Header("FoV Restriction Configuration")]
        [SerializeField] private InputActionProperty _toggleFovRestrictionAction; // right thumbstick press
        [SerializeField] private Transform _fovRestrictorPlane;
        [SerializeField] private float _defaultFovRestrictorSize = 1f; // restrictor size when standing still
        [SerializeField] private float _minFovRestrictorSize = .1f; // restrictor size when moving at maximum speed

        private bool _fovRestrictionEnabled = true;

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            if (GetComponent<NetworkObject>() != null) // If this script is attached to a networked UserPrefab
                if (!GetComponent<NetworkObject>().IsOwner) // And we are not the owner of this UserPrefab
                {
                    Destroy(_fovRestrictorPlane.gameObject); // We destroy the fov restrictor plane gameobject so it's not visible on remote avatars
                    Destroy(this); // We destroy this script (remove it from the prefab), because we don't want to steer remote users
                    return;
                }

            // Enabling the actions, so we can read input from them
            _leftHandSteeringAction.action.Enable();
            _rightHandSteeringAction.action.Enable();
            _toggleFovRestrictionAction.action.Enable();

            // FIX: delay snap by one frame so all colliders are fully initialized first
            StartCoroutine(SnapToGroundNextFrame());
        }

        // FIX: waits one frame before snapping, ensuring all scene colliders are ready
        private System.Collections.IEnumerator SnapToGroundNextFrame()
        {
            yield return null; // wait one frame

            // FIX: reset fall state before snapping so no stale falling data
            _isFalling = false;
            _fallStartTime = 0f;

            float targetHeight = GetTargetHeight();

            if (_groundFound) // FIX: only snap if raycast actually found ground
            {
                transform.position = new Vector3(
                    transform.position.x,
                    targetHeight,
                    transform.position.z
                );
                Debug.Log($"Snapped to ground at Y={targetHeight}");
            }
            else
            {
                Debug.LogWarning("SnapToGround: No ground found! Check Ground layer and collider setup.");
            }
        }

        private void Update()
        {
            // Reading the input depending on selected steeringHand as a float (how much the trigger is pressed) --> results in a float [0..1]
            float input = _steeringHand == SteeringHandedness.Left
                ? _leftHandSteeringAction.action.ReadValue<float>()
                : _rightHandSteeringAction.action.ReadValue<float>();

            if (input > 0) // apply steering, if button is pressed
                ApplySteeringInput(input);
            else
            {
                _currentSpeed = 0f; // if input is 0 --> current speed is zero
            }

            if (!_verticalSteering) // don't apply groundfollowing if user can steer vertically
                ApplyGroundFollowing();

            if (_toggleFovRestrictionAction.action.WasPressedThisFrame()) // toggle enabled state on button pressed
                ToggleFovRestriction();

            if (_fovRestrictionEnabled) // only apply fov restriction when enabled
                ApplyFovRestriction(input);
        }

        private void OnDestroy()
        {
            // FIX: clean up input actions when script is destroyed
            _leftHandSteeringAction.action.Disable();
            _rightHandSteeringAction.action.Disable();
            _toggleFovRestrictionAction.action.Disable();
        }

        #endregion

        #region Steering Methods

        // This method is used to apply the steering based on the user input
        private void ApplySteeringInput(float input)
        {
            _currentSpeed = input * _maxSteeringSpeed; // determining current speed
            float distance = _currentSpeed * Time.deltaTime; // determining move distance using Time.deltaTime to be frame-rate independent

            // steering direction (depending on selected steering hand) = forward (positive z-Axis) of selected forward indicator
            Vector3 handForward = _steeringHand == SteeringHandedness.Left
                ? _leftHandForwardIndicator.forward
                : _rightHandForwardIndicator.forward;

            Vector3 direction;
            if (!_verticalSteering)
            {
                // FIX: project onto horizontal plane directly instead of zeroing Y after
                // This preserves full movement speed regardless of controller tilt up/down
                direction = new Vector3(handForward.x, 0f, handForward.z);

                // Only skip movement if controller is pointing almost straight up/down
                if (direction.sqrMagnitude < 0.001f)
                    return;

                // Normalize AFTER flattening so speed is always consistent
                direction = direction.normalized;
            }
            else
            {
                direction = handForward.normalized;
            }

            transform.position += direction * distance;
        }

        #endregion

        #region Groundfollowing Methods

        // this method is used to apply the groundfollowing
        private void ApplyGroundFollowing()
        {
            float targetHeight = GetTargetHeight();

            // FIX: skip all ground following logic if raycast didn't hit valid ground
            // This prevents infinite falling when ground layer/collider is misconfigured
            if (!_groundFound)
                return;

            float heightDiff = targetHeight - transform.position.y; // get distance between target y-value and current y-value

            if (Mathf.Abs(heightDiff) < _adjustmentThreshold) // if difference < adjustment threshold --> just apply target height without animation
            {
                _isFalling = false; // reset helper variable
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z); // apply target value
            }
            else if (heightDiff < 0) // targetHeight below current height --> heightDiff is negative --> user is falling
            {
                if (!_isFalling) // is falling == false --> user just started to fall --> set helper variables for computation
                {
                    _isFalling = true;
                    _fallStartTime = Time.time;
                }

                // compute falling animation progress
                float fallTime = Time.time - _fallStartTime; // how long is user already falling
                // FIX: lowered cap from 100f to 20f to prevent overshooting through the floor
                Vector3 fallVec = Vector3.down * Mathf.Min(9.81f / 2f * Mathf.Pow(fallTime, 2f), 20f);

                transform.position += fallVec * Time.deltaTime; // apply falling frame-rate independent
            }
            else if (heightDiff > 0) // current position is below target height
            {
                float y = heightDiff * (_riseSpeed * Time.deltaTime); // calculate frame-rate independent vertical movement
                transform.position += new Vector3(0, y, 0); // apply vertical movement
            }
        }

        // this method performs the raycast to determine the y-value the user should be at
        private float GetTargetHeight()
        {
            // Start ray slightly above head position to avoid starting inside colliders
            Vector3 rayOrigin = _head.position + Vector3.up * 0.1f;

            // raycast straight down in world space from head position
            if (Physics.Raycast(_head.position, Vector3.down, out RaycastHit hit,
                Single.PositiveInfinity, _groundLayers))
            {
                _groundFound = true; // FIX: mark that valid ground was found
                Debug.Log($"Raycast hit: {hit.collider.gameObject.name} at Y={hit.point.y}");

                return hit.point.y; // FIX: offset by head height so XR Origin lands correctly
            }

            _groundFound = false; // FIX: mark that no ground was found
            Debug.LogWarning("Ground raycast hit nothing! Check Ground layer and collider setup.");
            return transform.position.y; // fallback: stay at current height
        }

        #endregion

        #region FoV Restriction Methods

        // this method is used to toggle if the fov restriction is enabled or not
        private void ToggleFovRestriction()
        {
            _fovRestrictionEnabled = !_fovRestrictionEnabled; // inverting enabled state

            // FIX: hide the plane immediately when toggled off
            if (!_fovRestrictionEnabled)
                _fovRestrictorPlane.gameObject.SetActive(false);

            // reseting restrictor scale, so it doesn't stay shrunk when deactivating
            _fovRestrictorPlane.localScale = new Vector3(_defaultFovRestrictorSize, _defaultFovRestrictorSize,
                _defaultFovRestrictorSize);
        }

        private void ApplyFovRestriction(float input)
        {
            // FIX: hide plane entirely when not moving instead of keeping it at default size
            if (input <= 0f)
            {
                _fovRestrictorPlane.gameObject.SetActive(false);
                return;
            }

            _fovRestrictorPlane.gameObject.SetActive(true);

            // determine required restrictor scale by lerping between the default (not active) size and minimum size when traveling at max speed
            // based on user input (input == 0 --> user stands still --> default size, input == 1 --> user moves at max speed --> minimum size)
            float newScale = Mathf.Lerp(_defaultFovRestrictorSize, _minFovRestrictorSize, input);

            // applying scale
            _fovRestrictorPlane.localScale = new Vector3(newScale, newScale, newScale);
        }

        #endregion
    }
}