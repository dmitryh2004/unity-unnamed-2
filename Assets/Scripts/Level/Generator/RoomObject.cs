using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] bool dontExtend = false;
    [SerializeField] List<RoomObject> neighbours = new();
    [SerializeField] List<GameObject> doorObjects = new();

    System.Random random = new System.Random();

    List<GameObject> doors = new();
    List<GameObject> doorWalls = new();

    [SerializeField] List<LootContainer> lootContainers = new();
    [SerializeField] int lootChance = 100;

    [SerializeField] GameObject coridorHorizontal, coridorVertical;
    [SerializeField] private float roomWeight = 0;
    [SerializeField] private float activityValue = 0;

    private void Awake()
    {
        foreach (Neighbour n in roomType.neighbours)
        {
            neighbours.Add(null);
        }

        if (roomType.hasDoors)
        {
            for (int i = 0; i < doorObjects.Count; i++)
            {
                doors.Add(doorObjects[i].transform.GetChild(0).gameObject);
                doorWalls.Add(doorObjects[i].transform.GetChild(1).gameObject);
            }
        }
    }

    public Vector3 GetCenter()
    {
        return transform.position;
    }

    public RoomObject GetNeighbour(int i)
    {
        if (i < 0 || i >= neighbours.Count) return null;
        return neighbours[i];
    }

    public bool CanHaveNeighbour(int dir)
    {
        if (dir < 0 || dir >= neighbours.Count) return false;
        if (GetNeighboursCount() >= roomType.maxNeighbours) return false;
        return true;
    }

    public int? SelectRandomUnusedDirection()
    {
        if (GetNeighboursCount() >= roomType.maxNeighbours) return null;
        int index = random.Next(0, neighbours.Count);
        while (neighbours[index] != null)
        {
            index++;
            if (index == neighbours.Count) index = 0;
        }
        return index;
    }

    /// <summary>
    /// Определяет, разблокирован ли проход в указанном направлении.
    /// </summary>
    /// <param name="dir">Номер проверяемого направления</param>
    /// <returns>true - проход существует и не заблокирован замком, false в противном случае</returns>
    public bool IsPathUnlocked(int dir)
    {
        if (dir < 0 || dir >= neighbours.Count) return false;
        bool res = HasNeighbour(dir);
        if (res)
        {
            bool hasDoor = doors[dir] != null && doors[dir].activeInHierarchy;
            if (hasDoor)
            {
                if (!roomType.canLockDoors) return true;
                return !(doors[dir].GetComponent<DoorManager>()?.GetDoorController().IsLocked() ?? true);
            }
            else return false;
        }
        else return false;
    }

    public int FindNeighbourIndex(RoomObject neighbour)
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i] == neighbour) return i;
        }
        return -1;
    }

    /// <summary>
    /// Проверяет, что между этой комнатой и соседней в заданном направлении существует незаблокированный проход.
    /// </summary>
    /// <param name="neighbourIndex">Номер проверяемого направления</param>
    /// <returns>true, если незаблокированный проход существует, иначе false</returns>
    public bool HasUnlockedPath(int neighbourIndex)
    {
        return IsPathUnlocked(neighbourIndex) && (neighbours[neighbourIndex]?.IsPathUnlocked(neighbours[neighbourIndex].FindNeighbourIndex(this)) ?? false);
    }

    /// <summary>
    /// Возвращает список комнат, доступных из текущей комнаты (существует незаблокированный проход к ним).
    /// </summary>
    /// <returns>Список доступных комнат.</returns>
    public List<RoomObject> GetAvailableRooms()
    {
        List<RoomObject> res = new ();
        Queue<RoomObject> temp = new ();
        List<RoomObject> checkedRooms = new ();
        temp.Enqueue(this);

        while (temp.Count > 0)
        {
            RoomObject currentRoom = temp.Dequeue();
            checkedRooms.Add(currentRoom);
            Debug.Log($"current room pos={currentRoom.GetCenter()}");
            if (currentRoom != this) res.Add(currentRoom);

            for (int i = 0; i < currentRoom.neighbours.Count; i++)
            {
                RoomObject neighbour = currentRoom.GetNeighbour(i);
                if (neighbour == null) continue;
                if (checkedRooms.Contains(neighbour)) continue;
                if (currentRoom.HasUnlockedPath(i))
                {
                    Debug.Log($"current room pos={currentRoom.GetCenter()} - unlocked path #{i} (to room pos={neighbour.GetCenter()})");
                    temp.Enqueue(neighbour);
                }
                else
                {
                    Debug.Log($"current room pos={currentRoom.GetCenter()} - locked path #{i} (to room pos={neighbour.GetCenter()})");
                }
            }
        }
        return res;
    }

    public bool IsPointOccupied(Vector3 point)
    {
        bool x = false, y = false, z = false;
        Vector3 center = GetCenter();
        x = (Mathf.Abs(point.x - center.x) <= roomType.width / 2f);
        y = ((point.y - center.y >= 0) && (point.y - center.y < roomType.height));
        z = (Mathf.Abs(point.z - center.z) <= roomType.length / 2f);

        return x && y && z;
    }

    public int GetNeighboursCount()
    {
        int res = 0;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (HasNeighbour(i)) res++;
        }
        return res;
    }

    public int GetMaxNeighboursCount()
    {
        return neighbours.Count;
    }

    public RoomScriptable GetRoomType() => roomType;
    public bool DontExtend() => dontExtend;

    public GameObject GetHorizontalCoridorPrefab() => coridorHorizontal;
    public GameObject GetVerticalCoridorPrefab() => coridorVertical;

    public bool HasNeighbour(int direction)
    {
        if (direction < 0 || direction >= neighbours.Count) throw new System.IndexOutOfRangeException();
        return (neighbours[direction] != null);
    }

    public void SetNeighbour(int direction, RoomObject neighbour)
    {
        if (direction < 0 || direction >= neighbours.Count) throw new System.IndexOutOfRangeException();
        neighbours[direction] = neighbour;
    }

    public void UpdateDoors()
    {
        if (roomType.hasDoors)
        {
            for (int i = 0; i < roomType.neighbours.Count; i++)
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

    public void ClampLockAlarmDifficulties()
    {
        for (int i = 0; i < roomType.neighbours.Count; i++)
        {
            List<LockController> locks = GetComponentsInChildren<LockController>().ToList();
            foreach (var _lock in locks)
            {
                int difficulty = _lock.GetDifficulty();
                _lock.SetAlarmDifficulty(difficulty + 1);
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

    public int SpawnLoot()
    {
        int sum = 0;
        foreach (LootContainer lc in lootContainers)
        {
            int chance = random.Next(1, 101);
            if (chance <= lootChance)
            {
                sum += lc.SpawnLoot();
            }
            else
            {
                if (lc.HideIfNoLoot())
                    lc.HideLootContainer();
            }
        }

        return sum;
    }

    public float RoomWeight => roomWeight;
    public void SetRoomWeight(float w) => roomWeight = w;
    public void ModifyRoomWeight(float diff) => roomWeight += diff;
    public float Activity => activityValue;
    public void ModifyActivity(float diff) => activityValue += diff;
}
