using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] DoorController door;

    public DoorController GetDoorController()
    {
        return door;
    }
}
