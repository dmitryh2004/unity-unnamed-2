using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class KVPComparer : IComparer<KeyValuePair<RoomObject, int>>
{
    public int Compare(KeyValuePair<RoomObject, int> x, KeyValuePair<RoomObject, int> y)
    {
        return x.Value - y.Value;
    }
}
public class LevelGenerator : MonoBehaviour
{
    System.Random random = new System.Random();
    [Header("Common settings")]
    [SerializeField] int roomsMin;
    [SerializeField] int roomsMax;
    [Space]
    [SerializeField] int newRoomOffsetStep = 9;
    [SerializeField] int maxJoinRange = 11;
    [Space(10)]
    [Header("Generation bounds")]
    [SerializeField] float minX = -100f;
    [SerializeField] float maxX = 100f;
    [Space]
    [SerializeField] float minY = -50f;
    [SerializeField] float maxY = 0f;
    [Space]
    [SerializeField] float minZ = -100f;
    [SerializeField] float maxZ = 100f;
    [Header("Exit room")]
    [SerializeField] bool generateExitRoom = true;
    [SerializeField] GameObject exitRoomPrefab;
    [Space(10)]
    [Header("Preplaced rooms")]
    [SerializeField] bool usePreplacedRooms = false;
    [SerializeField] List<RoomObject> preplacedRooms = new();
    [Space(10)]
    [Header("Possible rooms")]
    [SerializeField] List<GameObject> possibleRoomPrefabs = new();
    [SerializeField] List<int> possibleRoomPrefabsWeights = new();

    List<GameObject> roomPrefabs = new(); 
    List<KeyValuePair<RoomObject, int>> extensionCandidates = new();
    Dictionary<Vector3, RoomObject> generatedRooms = new();
    [SerializeField] GameObject coridorHorizontal, coridorVertical;

    int generatedLootSum = 0;
    int protectedRoomsCount = 0;
    int securedRoomsCount = 0;

    public int GetGeneratedLootSum() => generatedLootSum;
    public int GetProtectedRoomsCount() => protectedRoomsCount;
    public int GetSecuredRoomsCount() => securedRoomsCount;

    void Awake()
    {
        if (possibleRoomPrefabs.Count != possibleRoomPrefabsWeights.Count) return;

        for (int i = 0; i < possibleRoomPrefabs.Count; i++)
        {
            for (int j = 0; j < possibleRoomPrefabsWeights[i]; j++)
            {
                roomPrefabs.Add(possibleRoomPrefabs[i]);
            }
        }
    }

    public void Generate()
    {
        extensionCandidates.RemoveAll(x => true); // очищаем список на расширение
        generatedRooms.Clear(); // очищаем список сгенерированных комнат

        PlaceRooms(); // генерируем комнаты

        /*
        foreach (var room in generatedRooms.Values)
        {
            Debug.Log($"room {room.gameObject.name}: neighbours count={room.GetNeighboursCount()}, directions count={room.GetRoomType().neighbours.Count}");
        }
        */

        CreateCoridors(); // создаем коридоры между комнатами-соседями

        RoomPostGenerate(); // выполняем пост-генерационные действия для комнат (наполнение лутом)
        LevelPostGenerate(); // выполняет пост-генерационные действия для уровня в целом (запечка navmesh, активация охранников)
    }

    private void PlaceRooms()
    {
        int roomsRequired = random.Next(roomsMin, roomsMax + 1);
        int roomsCreated = 0;
        int failedAttempts = 0;

        if (generateExitRoom)
        {
            GameObject exitRoomGO = Instantiate(exitRoomPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform);
            RoomObject exitRoom = exitRoomGO.GetComponent<RoomObject>();

            generatedRooms.Add(Vector3.zero, exitRoom);
            extensionCandidates.Add(new KeyValuePair<RoomObject, int>(exitRoom, 1));

            roomsCreated++;
        }
        if (usePreplacedRooms)
        {
            foreach (RoomObject room in preplacedRooms)
            {
                generatedRooms.Add(room.transform.position, room);
                extensionCandidates.Add(new KeyValuePair<RoomObject, int>(room, room.GetRoomType().extensionPriority));

                RoomScriptable type = room.GetRoomType();

                if (type.isProtectedRoom) protectedRoomsCount++;
                if (type.isSecuredRoom) securedRoomsCount++;
            }

            UpdateNeighbours();
            UpdateCandidates();
        }

        while (roomsCreated < roomsRequired && failedAttempts < roomsRequired * 2)
        {
            int sum = 0;
            foreach(var kvp in extensionCandidates)
            {
                sum += kvp.Value;
            }

            int index = random.Next(0, sum);
            RoomObject extended = null;
            sum = 0;

            for (int i = 0; i < extensionCandidates.Count; i++)
            {
                sum += extensionCandidates[i].Value;
                if (sum > index)
                {
                    extended = extensionCandidates[i].Key;
                    break;
                }
            }

            if (extended == null)
            {
                Debug.Log("Level generator - nothing to extend");
                return;
            }

            Debug.Log(extended.DontExtend());
            
            RoomScriptable extendedType = extended.GetRoomType();
            int? direction = extended.SelectRandomUnusedDirection();
            if (direction != null)
            {
                Vector3 center = extended.GetCenter();
                Vector3 typeSpawnOffset = extendedType.neighbours[direction.Value].spawnOffset;
                Directions oppositeDirection = DirectionsController.GetOppositeDirection(extendedType.neighbours[direction.Value].direction);

                int offsetX = (int)typeSpawnOffset.x, offsetY = (int)typeSpawnOffset.y, offsetZ = (int)typeSpawnOffset.z;
                int gridOffsetX = newRoomOffsetStep * ((Mathf.Abs(offsetX) > Mathf.Abs(offsetZ)) ? (offsetX > 0 ? 1 : -1) : 0), 
                    gridOffsetY = offsetY, 
                    gridOffsetZ = newRoomOffsetStep * ((Mathf.Abs(offsetZ) > Mathf.Abs(offsetX)) ? (offsetZ > 0 ? 1 : -1) : 0);

                center += new Vector3(gridOffsetX, gridOffsetY, gridOffsetZ);
                List<KeyValuePair<GameObject, int>> matching = new();
                foreach (GameObject prefab in roomPrefabs)
                {
                    RoomScriptable type = prefab.GetComponent<RoomObject>().GetRoomType();
                    bool res = true;
                    if (type.useInputDirection)
                    {
                        res = res && type.neighbours[type.inputDirection].direction == oppositeDirection;
                    }
                    if (extensionCandidates.Count == 1)
                    {
                        res = res && (type.maxNeighbours > 1);
                    }

                    if (res)
                    {
                        for (int i = 0; i < type.neighbours.Count; i++)
                        {
                            Neighbour n = type.neighbours[i];
                            if (n.direction == oppositeDirection)
                            {
                                if (IsEnoughSpace(center, type, i))
                                {
                                    matching.Add(new KeyValuePair<GameObject, int>(prefab, i));
                                }
                            }
                        }
                    }
                }

                if (matching.Count > 0)
                {
                    var selected = matching[random.Next(0, matching.Count)];

                    GameObject selectedPrefab = selected.Key;
                    RoomScriptable type = selectedPrefab.GetComponent<RoomObject>().GetRoomType();

                    int createdRoomDirection = selected.Value;

                    center += Vector3.up * (type.spawnHeightOffset - type.neighbours[createdRoomDirection].spawnOffset.y);

                    if (type.isProtectedRoom) protectedRoomsCount++;
                    if (type.isSecuredRoom) securedRoomsCount++;

                    GameObject createdRoom = Instantiate(selectedPrefab, center, Quaternion.Euler(0, 0, 0), transform);
                    RoomObject createdRoomObject = createdRoom.GetComponent<RoomObject>();

                    extended.SetNeighbour((int)direction, createdRoomObject);
                    createdRoomObject.SetNeighbour(createdRoomDirection, extended);

                    generatedRooms.Add(center, createdRoomObject);
                    extensionCandidates.Add(new KeyValuePair<RoomObject, int>(createdRoomObject, createdRoomObject.GetRoomType().extensionPriority));

                    UpdateNeighbours();
                    roomsCreated++;
                }
                else
                {
                    failedAttempts++;
                }
            }
            //roomsCreated++;
            UpdateCandidates();

            if (extensionCandidates.Count == 0) break;
        }

        if (failedAttempts == roomsRequired * 2)
        {
            Debug.LogWarning($"Failed to spawn {roomsRequired} rooms (only {roomsCreated} spawned). Generation bounds maybe too narrow.");
        }
    }

    private bool IsEnoughSpace(Vector3 _center, RoomScriptable type, int inputDirection)
    {
        Vector3 center = _center - type.neighbours[inputDirection].spawnOffset;
        int sizeX = type.width, sizeY = type.height, sizeZ = type.length;
        int heightOffset = type.spawnHeightOffset;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j <= sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    Vector3 coords = new Vector3(
                            center.x - sizeX / 2f + i,
                            center.y + j,
                            center.z - sizeZ / 2f + k
                        );
                    if (coords.x < minX || coords.x > maxX) return false;
                    if (coords.y < minY || coords.y > maxY) return false;
                    if (coords.z < minZ || coords.z > maxZ) return false;
                    foreach (RoomObject room in generatedRooms.Values)
                    {
                        if (room.IsPointOccupied(coords)) return false;
                    }
                }
            }
        }
        return true;
    }

    private void UpdateNeighbours()
    {
        foreach (RoomObject room in generatedRooms.Values)
        {
            if (room.GetNeighboursCount() >= room.GetRoomType().maxNeighbours) continue;
            // find new neighbours
            Vector3 center = room.GetCenter();
            RoomScriptable type = room.GetRoomType();

            for (int i = 0; i < type.neighbours.Count; i++)
            {
                if (room.HasNeighbour(i)) continue;
                if (room.GetNeighboursCount() >= room.GetRoomType().maxNeighbours) continue;
                CheckNeighbourDirection(center, type, i, room);
            }
        }
    }

    private void CheckNeighbourDirection(Vector3 center, RoomScriptable type, int i, RoomObject room)
    {
        Neighbour neighbour = type.neighbours[i];
        Vector3 offset = neighbour.spawnOffset;
        Directions dir = neighbour.direction;
        Vector3 searchDirection = new Vector3(
            (dir == Directions.east) ? 1 : ((dir == Directions.west) ? -1 : 0),
            0,
            (dir == Directions.north) ? 1 : ((dir == Directions.south) ? -1 : 0)
        );
        for (int currentRange = 0; currentRange < maxJoinRange; currentRange++)
        {
            RoomObject possibleNeighbour = null;
            foreach (RoomObject existingRoom in generatedRooms.Values)
            {
                if (existingRoom == room) continue;
                if (existingRoom.IsPointOccupied(center + offset + searchDirection * currentRange))
                {
                    possibleNeighbour = existingRoom;
                    break;
                }
            }

            if (possibleNeighbour == null) continue;

            if (possibleNeighbour.GetNeighboursCount() >= possibleNeighbour.GetRoomType().maxNeighbours) return;

            List<Neighbour> possibleNeighbourNeighbours = possibleNeighbour.GetRoomType().neighbours;
            for (int j = 0; j < possibleNeighbourNeighbours.Count; j++)
            {
                if (possibleNeighbour.HasNeighbour(j)) continue;
                Neighbour possibleNeighbourN = possibleNeighbourNeighbours[j];
                Debug.Log($"cycle coords: {center + offset + searchDirection * currentRange}, pn coords: {possibleNeighbour.GetCenter() + possibleNeighbourN.spawnOffset}");
                if (center + offset + searchDirection * currentRange == possibleNeighbour.GetCenter() + possibleNeighbourN.spawnOffset)
                {
                    room.SetNeighbour(i, possibleNeighbour);
                    possibleNeighbour.SetNeighbour(j, room);
                    Debug.Log($"{room.gameObject.name}: found new neighbour at direction {i} ({possibleNeighbour.gameObject.name})");
                    return;
                }
            }

            break;
        }
    }

    private void UpdateCandidates()
    {
        List<RoomObject> removalList = new();
        foreach(KeyValuePair<RoomObject, int> kvp in extensionCandidates)
        {
            RoomObject room = kvp.Key;
            bool remove = room.DontExtend(); // проверка условий непродления этой комнаты
            remove = remove || (room.GetNeighboursCount() == room.GetRoomType().maxNeighbours);
            if (remove) removalList.Add(room);
        }

        foreach(RoomObject removed in removalList)
        {
            extensionCandidates.RemoveAll(x => x.Key == removed);
        }

        extensionCandidates.Sort(new KVPComparer());
    }

    private void CreateCoridors()
    {
        // generate coridors only on north and east directions for avoiding collisions
        foreach (RoomObject room in generatedRooms.Values)
        {
            Vector3 roomCenter = room.GetCenter();
            
            for (int i = 0; i < room.GetRoomType().neighbours.Count; i++)
            {
                Neighbour n = room.GetRoomType().neighbours[i];
                Vector3 offset = Vector3.zero;
                switch(n.direction)
                {
                    case Directions.north:
                        offset = Vector3.forward;
                        break;
                    case Directions.east:
                        offset = Vector3.right;
                        break;
                    default:
                        continue;
                }

                Vector3 curPos = roomCenter + n.spawnOffset + offset * 0.5f;
                GameObject usingPrefab = null;
                if (n.direction == Directions.north)
                {
                    usingPrefab = (room.GetVerticalCoridorPrefab() != null) ? room.GetVerticalCoridorPrefab() : coridorVertical;
                }
                else
                {
                    usingPrefab = (room.GetHorizontalCoridorPrefab() != null) ? room.GetHorizontalCoridorPrefab() : coridorHorizontal;
                }

                RoomObject neighbour = room.GetNeighbour(i);
                if (neighbour != null)
                {
                    int count = 0;
                    while (!neighbour.IsPointOccupied(curPos) && count < maxJoinRange)
                    {
                        Instantiate(usingPrefab, curPos, Quaternion.Euler(0, 0, 0), transform);
                        count++;
                        curPos += offset;
                    }
                }
            }
        }
    }

    private void RoomPostGenerate()
    {
        float changeAlarmDifficultyChance = AdaptiveDifficultyManager.Instance?.Values.GetParameterValue("ChangeLockRaiseAlarmDifficultyChance", AdaptiveDifficultyManager.Instance.AlertnessDegree()) ?? 0;
        bool changeAlarmDifficulty = false;
        if (random.Next(0, 100) < changeAlarmDifficultyChance * 100)
        {
            changeAlarmDifficulty = true;
        }

        int lootSum = 0;
        foreach (RoomObject room in generatedRooms.Values)
        {
            room.UpdateDoors();
            if (changeAlarmDifficulty)
            {
                room.ClampLockAlarmDifficulties();
            }
            lootSum += room.SpawnLoot();
        }

        generatedLootSum = lootSum;

        Debug.Log($"Generated loot with total cost = {generatedLootSum}");

        FindFirstObjectByType<PlayerScannerController>()?.FindLootCostHints();
    }

    private void LevelPostGenerate()
    {
        // bake nav mesh
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        // apply weights for rooms from adaptive difficulty manager
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            AdaptiveDifficultyManager.Instance.ApplyRoomWeights(generatedRooms.Values.ToList());

        // init preplaced guardians
        GuardianController[] guardianControllers = FindObjectsByType<GuardianController>(FindObjectsSortMode.None);
        foreach (var guardian in guardianControllers)
        {
            guardian.Init();
        }
    }

    public RoomObject GetRoomByPosition(Vector3 position, float precision)
    {
        foreach (var item in generatedRooms)
        {
            Vector3 key = item.Key;
            if (Vector3.Distance(key, position) < precision)
            {
                return item.Value;
            }
        }
        return null;
    }

    public void UpdateRoomWeights()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            AdaptiveDifficultyManager.Instance.UpdateRoomWeights(generatedRooms.Values.ToList());
    }
}
