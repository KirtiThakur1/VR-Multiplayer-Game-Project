using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VRSYS.Core.Utility;

namespace VRSYS.Core.Navigation
{
    public class HMDSteeringNavigation_Fixed : MonoBehaviour
    {
        [Header("Core References")]
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

            if (navigationBounds != null)
                KeepInsideBounds();
        }

        private void ApplyMovement()
        {

            float forward = rightSteeringAction.action?.ReadValue<float>() ?? 0f; // right trigger
            float backward = leftSteeringAction.action?.ReadValue<float>() ?? 0f; // left trigger

            float speed = forward - backward;   // >0 forward, <0 backward
            if (Mathf.Abs(speed) < 0.01f) return;

            Vector3 direction = head.forward;
            direction.y = 0f;
            direction.Normalize();

            transform.position += direction * speed * steeringSpeed * Time.deltaTime;
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
        }

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
