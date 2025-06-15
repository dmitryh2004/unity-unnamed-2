using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    north = 0,
    west = 1,
    south = 2,
    east = 3
}

public class RoomObject : MonoBehaviour
{
    [SerializeField] RoomScriptable roomType;
    RoomObject[] neighbours = new RoomObject[4];

    private void Start()
    {
        neighbours[0] = null;
        neighbours[1] = null;
        neighbours[2] = null;
        neighbours[3] = null;
    }

    
}
