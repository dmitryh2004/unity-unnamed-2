using System;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    north = 0,
    west = 1,
    south = 2,
    east = 3
}

public static class DirectionsController
{
    public static Directions GetOppositeDirection(Directions dir)
    {
        if (dir == Directions.north) return Directions.south;
        if (dir == Directions.west) return Directions.east;
        if (dir == Directions.south) return Directions.north;
        if (dir == Directions.east) return Directions.west;
        return Directions.north;
    }
}

public class RoomObject : MonoBehaviour
{
    [SerializeField] RoomScriptable roomType;
    RoomObject[] neighbours = new RoomObject[4];
    [SerializeField] GameObject northDoor, westDoor, southDoor, eastDoor;

    System.Random random = new System.Random();

    bool[] possibleDirections = new bool[4];
    GameObject[] doors = new GameObject[4];
    GameObject[] doorWalls = new GameObject[4];

    private void Awake()
    {
        neighbours[0] = null;
        neighbours[1] = null;
        neighbours[2] = null;
        neighbours[3] = null;

        possibleDirections[0] = roomType.north;
        possibleDirections[1] = roomType.west;
        possibleDirections[2] = roomType.south;
        possibleDirections[3] = roomType.east;

        if (roomType.hasDoors)
        {
            List<GameObject> dirs = new();
            dirs.Add(northDoor);
            dirs.Add(westDoor);
            dirs.Add(southDoor);
            dirs.Add(eastDoor);

            for (int i = 0; i < 4; i++)
            {
                if (dirs[i] != null)
                {
                    doors[i] = dirs[i].transform.GetChild(0).gameObject;
                    doorWalls[i] = dirs[i].transform.GetChild(1).gameObject;
                }
            }
        }
    }

    public Vector3 GetCenter()
    {
        return transform.position;
    }

    public RoomObject GetNeighbour(int i)
    {
        if (i < 0 || i > 3) return null;
        return neighbours[i];
    }

    public bool CanHaveNeighbour(int dir)
    {
        if (dir < 0 || dir > 3) return false;
        return possibleDirections[dir];
    }

    public Directions? SelectRandomUnusedDirection()
    {
        if (GetNeighboursCount() >= roomType.maxNeighbours) return null;
        int index = random.Next(0, 4);
        while (neighbours[index] != null || !possibleDirections[index])
        {
            index++;
            if (index == 4) index = 0;
        }
        return (Directions)index;
    }

    public bool IsPointOccupied(Vector3 point)
    {
        bool x = false, y = false, z = false;
        Vector3 center = GetCenter();
        x = (Mathf.Abs(point.x - center.x) < roomType.width / 2f);
        y = ((point.y - center.y >= 0) && (point.y - center.y < roomType.height));
        z = (Mathf.Abs(point.z - center.z) < roomType.length / 2f);

        return x && y && z;
    }

    public int GetNeighboursCount()
    {
        int res = 0;
        for (int i = 0; i < 4; i++)
        {
            if (HasNeighbour(i)) res++;
        }
        return res;
    }

    public RoomScriptable GetRoomType() => roomType;

    public bool HasNeighbour(int direction)
    {
        if (direction < 0 || direction > 3) throw new System.IndexOutOfRangeException();
        return (neighbours[direction] != null);
    }

    public bool HasNeighbour(Directions direction)
    {
        return HasNeighbour((int)direction);
    }

    public void SetNeighbour(int direction, RoomObject neighbour)
    {
        if (direction < 0 || direction > 3) throw new System.IndexOutOfRangeException();
        neighbours[direction] = neighbour;
    }

    public void SetNeighbour(Directions direction, RoomObject neighbour)
    {
        SetNeighbour((int)direction, neighbour);
    }

    public void UpdateDoors()
    {
        if (roomType.hasDoors)
        {
            for (int i = 0; i < 4; i++)
            {
                if (possibleDirections[i])
                {
                    bool active = HasNeighbour(i);
                    doors[i].SetActive(active);
                    doorWalls[i].SetActive(!active);

                    if (active)
                    {
                        if (roomType.canLockDoors) // если в комнате могут быть запертые двери
                        {
                            int chance = random.Next(1, 101); // берем рандомное число [1; 100]
                            if (chance > roomType.lockChance) // если оно больше заданного в настройках процента
                            {
                                // убираем замок
                                RemoveLockFromDoor(i);
                            }
                            else // иначе
                            {
                                // задаем рандомную сложность замку в пределах заданной в настройках
                                SetRandomStartDifficultyForDoor(i);
                            }
                        }
                        else // если в комнате не может быть запертых дверей
                        {
                            // убираем замок
                            RemoveLockFromDoor(i);
                        }
                    }
                }
            }
        }
    }

    void SetRandomStartDifficultyForDoor(int direction)
    {
        DoorManager door;
        if (doors[direction].TryGetComponent(out door))
        {
            int difficulty = random.Next(roomType.lockStartDifficultyMin, roomType.lockStartDifficultyMax + 1);
            door.GetDoorController().GetLocker().SetDifficulty(difficulty);
        }
    }

    void RemoveLockFromDoor(int direction)
    {
        DoorManager door;
        if (doors[direction].TryGetComponent(out door))
        {
            door.GetDoorController().GetLocker().RemoveLock();
        }
    }
}
