using UnityEngine;

[CreateAssetMenu(fileName = "RoomScriptable", menuName = "Scriptable Objects/Generator/RoomScriptable")]
public class RoomScriptable : ScriptableObject
{
    public int id;
    public int length;
    public int width;
    public int height;

    [Header("Spawn directions")]
    public bool north;
    public int northHeightOffset = 0;
    [Space(10)]
    public bool west;
    public int westHeightOffset = 0;
    [Space(10)]
    public bool south;
    public int southHeightOffset = 0;
    [Space(10)]
    public bool east;
    public int eastHeightOffset = 0;

    [Header("Spawn settings")]
    [Range(1, 4)] public int minNeighbours = 1;
    [Range(1, 4)] public int maxNeighbours = 4;

    [Header("Doors settings")]
    public bool hasDoors;
    public bool canLockDoors;
}
