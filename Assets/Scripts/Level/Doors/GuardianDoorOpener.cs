using UnityEngine;

public class GuardianDoorOpener : MonoBehaviour
{
    [SerializeField] DoorController doorController;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{gameObject.name}: {other.name} entered the trigger");
        GuardianController gc;
        if (other.TryGetComponent(out gc))
        {
            if (gc.CanOpenClosedDoors())
            {
                if (!doorController.IsLocked() && !doorController.IsOpen())
                {
                    doorController.Interact();
                    GuardianManager.Instance.UpdateInteractableState(doorController, doorController.IsOpen());
                }
            }
        }
    }
}
