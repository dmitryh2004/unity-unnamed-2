using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Neighbour
{
    public Directions direction;
    public Vector3 spawnOffset;
}

[CreateAssetMenu(fileName = "RoomScriptable", menuName = "Scriptable Objects/Generator/RoomScriptable")]
public class RoomScriptable : ScriptableObject
{
    public int id;
    public int length;
    public int width;
    public int height;

    public int extensionPriority = 1;

    [Space(10)]
    public int spawnHeightOffset = 0;

    [Header("Spawn directions")]
    public List<Neighbour> neighbours = new();

    [Space(10)]
    public bool useInputDirection = false;
    public int inputDirection;

    [Header("Spawn settings")]
    public int minNeighbours = 1;
    public int maxNeighbours = 4;

    [Header("Doors settings")]
    public bool hasDoors;
    public bool canLockDoors;
    public int lockChance = 100;
    public int lockStartDifficultyMin = 1;
    public int lockStartDifficultyMax = 3;

    [Header("Statistics")]
    public bool isProtectedRoom = false;
    public bool isSecuredRoom = false;
}
