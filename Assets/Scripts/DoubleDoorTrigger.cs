using UnityEngine;

public class DoubleDoorTrigger : MonoBehaviour
{
    public DoorController leftDoor;
    public DoorController rightDoor;
    public bool closeOnExit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (leftDoor != null) leftDoor.Open();
        if (rightDoor != null) rightDoor.Open();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!closeOnExit) return;
        if (!other.CompareTag("Player")) return;

        if (leftDoor != null) leftDoor.Close();
        if (rightDoor != null) rightDoor.Close();
    }
}
