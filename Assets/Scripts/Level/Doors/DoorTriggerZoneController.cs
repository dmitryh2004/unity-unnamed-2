using UnityEngine;

public class DoorTriggerZoneController : MonoBehaviour
{
    [SerializeField] DoorController doorController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            doorController.SetInTriggerZone(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            doorController.SetInTriggerZone(false);
    }
}
