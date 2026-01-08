using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VRSYS.Core.Utility;



namespace VRSYS.Core.Navigation
{
    public class HMDSteeringNavigation_Fixed : MonoBehaviour
    {

        [Header("Core References")]

        [Tooltip("Reference to the head transform (usually Main Camera)")]
        public Transform head;
        public HandType steeringHand = HandType.Left;

        [Header("Input Actions")]
        public InputActionProperty leftSteeringAction;   // Trigger
        public InputActionProperty rightSteeringAction;  // Trigger
        public InputActionProperty leftTurnAction;       // Joystick X
        public InputActionProperty rightTurnAction;      // Joystick X

        [Header("Steering Settings")]
        public float steeringSpeed = 3f;
        public float rotationSpeed = 30f;
        public bool verticalSteering = false;

        [Header("Ground Following")]
        [Tooltip("Layer mask for surfaces the player should walk on")]
        [SerializeField] private LayerMask groundLayer;

        [Tooltip("Height of the player above ground")]
        [SerializeField] private float playerHeight = 0.0f;

        [Tooltip("Distance to keep above the ground (prevents clipping)")]
        [SerializeField] private float groundOffset = 0.1f;

        [Tooltip("Maximum distance to raycast downward looking for ground")]
        [SerializeField] private float maxGroundCheckDistance = 10f;

        [Tooltip("Enable/disable ground following")]
        [SerializeField] private bool enableGroundFollowing = true;


        [Header("Optional")]
        public NavigationBounds navigationBounds;

        [Header("Debug")]
        public bool debugLogs = true;

        private NetworkObject netObj;

        private void Awake()
        {
            netObj = GetComponent<NetworkObject>();
        }

        private void OnEnable()
        {
            leftSteeringAction.action?.Enable();
            rightSteeringAction.action?.Enable();
            leftTurnAction.action?.Enable();
            rightTurnAction.action?.Enable();
        }

        private void Start()
        {
            // Network ownership safety
            if (netObj != null && !netObj.IsOwner)
            {
                enabled = false;
                return;
            }

            if (debugLogs)
                Debug.Log("[HMD Steering FIXED] Initialized");
        }

        private void Update()
        {
            ApplyMovement();
            ApplyRotation();

            // Applying ground following even when not moving
            if (enableGroundFollowing)
            {
                ApplyGroundFollowing();
            }

            if (navigationBounds != null)
                KeepInsideBounds();
        }

        private void ApplyMovement()
        {

            float forward = rightSteeringAction.action?.ReadValue<float>() ?? 0f; // right trigger
            float backward = leftSteeringAction.action?.ReadValue<float>() ?? 0f; // left trigger

            float speed = forward - backward;   // >0 forward, <0 backward
            if (Mathf.Abs(speed) < 0.01f) return;

            //horizontal movement direction
            Vector3 direction = head.forward;
            direction.y = 0f;
            direction.Normalize();

            //transform.position += direction * speed * steeringSpeed * Time.deltaTime;

            // new horizontal position
            //Vector3 newPosition = transform.position + direction * speed * steeringSpeed * Time.deltaTime;
            // Apply horizontal movement only
            Vector3 horizontalMove = direction * speed * steeringSpeed * Time.deltaTime;
            transform.position += new Vector3(horizontalMove.x, 0f, horizontalMove.z);
        }

        //  GROUND FOLLOWING
        private void ApplyGroundFollowing()
        {
            // Cast ray downward from head to find ground
            Ray ray = new Ray(head.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, maxGroundCheckDistance, groundLayer))
            {
                // Use fixed player height instead of actual head height
                float targetY = hit.point.y + playerHeight + groundOffset;

                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

                if (debugLogs)
                {
                    Debug.DrawLine(head.position, hit.point, Color.green, 0.1f);
                }
            }
            else
            {
                if (debugLogs)
                {
                    Debug.DrawRay(head.position, Vector3.down * maxGroundCheckDistance, Color.red, 0.1f);
                }
            }
        }
        /*float speed = 0f;

        // --- READ TRIGGER ---
        if (steeringHand == HandType.Left)
            speed = leftSteeringAction.action?.ReadValue<float>() ?? 0f;
        else
            speed = rightSteeringAction.action?.ReadValue<float>() ?? 0f;

        if (speed < 0.01f)
            return;

        if (debugLogs)
            Debug.Log($"[HMD Steering] trigger={speed:F2}, isOwner={netObj?.IsOwner}");


        /* // TEMP TEST: always walk forward
         Vector3 direction = head.forward;
         direction.y = 0f;
         direction.Normalize();

         transform.position += direction * steeringSpeed * Time.deltaTime; */

        // --- THIS IS THE IMPORTANT FIX ---
        /*Vector3 direction = head.forward;   //  HEAD FORWARD
        direction.y = 0f;                   //  FLATTEN
        direction.Normalize();              //  NORMALIZE

        Vector3 move =
            direction * speed * steeringSpeed * Time.deltaTime;

        transform.position += move;

        if (debugLogs)
            Debug.Log($"[Move] speed={speed:F2} dir={direction}");  */


        private void ApplyRotation()
        {
            float turn = 0f;

            if (steeringHand == HandType.Left)
                turn = leftTurnAction.action?.ReadValue<Vector2>().x ?? 0f;
            else
                turn = rightTurnAction.action?.ReadValue<Vector2>().x ?? 0f;

            if (Mathf.Abs(turn) < 0.01f)
                return;

            transform.RotateAround(
                head.position,
                Vector3.up,
                turn * rotationSpeed * Time.deltaTime
            );
        }

        private void KeepInsideBounds()
        {
            if (navigationBounds.bounds.Contains(head.position))
                return;

            Vector3 closest =
                navigationBounds.collider.ClosestPointOnBounds(head.position);

            transform.position += closest - head.position;
        }
    }
}
