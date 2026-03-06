using Unity.Netcode;
using UnityEngine;

public class ServerProximityDoubleDoor : NetworkBehaviour
{
    [Header("Door Leaves (the objects that rotate)")]
    public Transform leftDoor;   
    public Transform rightDoor;  

    [Header("Trigger / Detection")]
    public Transform triggerCenter;     
    public float openDistance = 2.0f;   

    [Header("Rotation (Local)")]
    public Vector3 leftClosedEuler;
    public Vector3 leftOpenEuler;

    public Vector3 rightClosedEuler;
    public Vector3 rightOpenEuler;

    [Header("Motion")]
    public float rotateSpeed = 6f; 

    void Reset()
    {
        
        openDistance = 2f;
        rotateSpeed = 6f;

        leftClosedEuler = Vector3.zero;
        rightClosedEuler = Vector3.zero;

        leftOpenEuler = new Vector3(0f, -90f, 0f);
        rightOpenEuler = new Vector3(0f, 90f, 0f);
    }

    void Update()
    {
        if (!IsServer) return; // 

        if (!leftDoor || !rightDoor) return;

        Vector3 center = triggerCenter ? triggerCenter.position : transform.position;

        bool shouldOpen = AnyPlayerWithinDistance(center, openDistance);

        Quaternion leftTarget = Quaternion.Euler(shouldOpen ? leftOpenEuler : leftClosedEuler);
        Quaternion rightTarget = Quaternion.Euler(shouldOpen ? rightOpenEuler : rightClosedEuler);

        float k = 1f - Mathf.Exp(-rotateSpeed * Time.deltaTime);

        leftDoor.localRotation = Quaternion.Slerp(leftDoor.localRotation, leftTarget, k);
        rightDoor.localRotation = Quaternion.Slerp(rightDoor.localRotation, rightTarget, k);
    }

    bool AnyPlayerWithinDistance(Vector3 center, float dist)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return false;

        float distSqr = dist * dist;

        foreach (var client in nm.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;
            if (playerObj == null) continue;

            Vector3 p = playerObj.transform.position;

            if ((p - center).sqrMagnitude <= distSqr)
                return true;
        }

        return false;
    }
}