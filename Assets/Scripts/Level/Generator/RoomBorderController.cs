using UnityEngine;

public class RoomBorderController : MonoBehaviour
{
    RoomEventManager eventManager = null;
    private void Start()
    {
        eventManager = GetComponentInParent<RoomEventManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            eventManager?.RoomEnterEvent();
        }
    }
}
