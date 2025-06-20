using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    System.Random random = new System.Random();
    [SerializeField] int roomsMin, roomsMax;
    [SerializeField] int gridStep = 9;
    [SerializeField] GameObject exitRoomPrefab;
    [SerializeField] List<GameObject> possibleRoomPrefabs = new();
    [SerializeField] List<int> possibleRoomPrefabsWeights = new();

    List<GameObject> roomPrefabs = new(); 
    List<RoomObject> extensionCandidates = new();
    Dictionary<Vector3, RoomObject> generatedRooms = new();
    [SerializeField] GameObject coridorHorizontal, coridorVertical;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (possibleRoomPrefabs.Count != possibleRoomPrefabsWeights.Count) return;

        for (int i = 0; i < possibleRoomPrefabs.Count; i++)
        {
            for (int j = 0; j < possibleRoomPrefabsWeights[i]; j++)
            {
                roomPrefabs.Add(possibleRoomPrefabs[i]);
            }
        }
        Generate();
    }

    void Generate()
    {
        extensionCandidates.RemoveAll(x => true);
        generatedRooms.Clear();
        PlaceRooms();
        CreateCoridors();
        UpdateRoomDoors();
        BakeNavMesh();
    }

    private void PlaceRooms()
    {
        int roomsRequired = random.Next(roomsMin, roomsMax + 1);
        int roomsCreated = 1;
        GameObject exitRoomGO = Instantiate(exitRoomPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform);
        RoomObject exitRoom = exitRoomGO.GetComponent<RoomObject>();

        generatedRooms.Add(Vector3.zero, exitRoom);
        extensionCandidates.Add(exitRoom);

        while (roomsCreated < roomsRequired)
        {
            RoomObject extended = extensionCandidates[random.Next(0, extensionCandidates.Count)];
            RoomScriptable extendedType = extended.GetRoomType();
            Directions? direction = extended.SelectRandomUnusedDirection();
            if (direction != null)
            {
                Vector3 center = extended.GetCenter();
                int offsetX = 0, offsetY = 0, offsetZ = 0;
                switch (direction)
                {
                    case Directions.north:
                        offsetZ += gridStep;
                        offsetY = extendedType.northHeightOffset;
                        break;
                    case Directions.south:
                        offsetZ -= gridStep;
                        offsetY = extendedType.southHeightOffset;
                        break;
                    case Directions.west:
                        offsetX -= gridStep;
                        offsetY = extendedType.westHeightOffset;
                        break;
                    case Directions.east:
                        offsetX += gridStep;
                        offsetY = extendedType.eastHeightOffset;
                        break;
                }

                center += new Vector3(offsetX, offsetY, offsetZ);
                List<GameObject> matching = roomPrefabs.FindAll(x => {
                    RoomScriptable type = x.GetComponent<RoomObject>().GetRoomType();
                    bool res = IsEnoughSpace(center, type);
                    if (type.useInputDirection)
                    {
                        res = res && type.inputDirection == DirectionsController.GetOppositeDirection((Directions)direction);
                    }
                    return res;
                });

                if (matching.Count > 0)
                {
                    GameObject selectedPrefab = matching[random.Next(0, matching.Count)];
                    
                    // calculate object height (Y) offset
                    RoomScriptable type = selectedPrefab.GetComponent<RoomObject>().GetRoomType();
                    center += Vector3.up * type.spawnHeightOffset;

                    GameObject createdRoom = Instantiate(selectedPrefab, center, Quaternion.Euler(0, 0, 0), transform);
                    RoomObject createdRoomObject = createdRoom.GetComponent<RoomObject>();

                    extended.SetNeighbour((int)direction, createdRoomObject);
                    createdRoomObject.SetNeighbour((int)DirectionsController.GetOppositeDirection((Directions)direction), extended);

                    generatedRooms.Add(center, createdRoomObject);
                    extensionCandidates.Add(createdRoomObject);

                    UpdateNeighbours();
                    roomsCreated++;
                }
                
            }
            UpdateCandidates();

            if (extensionCandidates.Count == 0) break;
        }
    }

    private bool IsEnoughSpace(Vector3 center, RoomScriptable type)
    {
        int sizeX = type.width, sizeY = type.height, sizeZ = type.length;
        int heightOffset = type.spawnHeightOffset;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    Vector3 coords = new Vector3(
                            center.x - sizeX / 2f + i,
                            center.y + heightOffset + j,
                            center.z - sizeZ / 2f + k
                        );
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
            //add links to neighbours of room
            for (int i = 0; i < 4; i++)
            {
                if (room.CanHaveNeighbour(i))
                {
                    RoomObject neighbour = room.GetNeighbour(i);
                    if (neighbour != null)
                    {
                        int index = (i + 2) % 4;
                        if (neighbour.CanHaveNeighbour(index) && neighbour.GetNeighbour(index) != room)
                        {
                            neighbour.SetNeighbour(index, room);
                        }
                    }
                }
            }

            // find new neighbours
            Vector3 center = room.GetCenter();
            RoomObject northNeighbour, westNeighbour, southNeighbour, eastNeighbour;
            if (!room.HasNeighbour(0) && generatedRooms.TryGetValue(center + gridStep * new Vector3(0,0,1) + room.GetRoomType().northHeightOffset * Vector3.up, out northNeighbour))
            {
                if (room.CanHaveNeighbour(0) && northNeighbour.CanHaveNeighbour(2))
                    room.SetNeighbour(Directions.north, northNeighbour);
            }
            if (!room.HasNeighbour(1) && generatedRooms.TryGetValue(center + gridStep * new Vector3(-1, 0, 0) + room.GetRoomType().westHeightOffset * Vector3.up, out westNeighbour))
            {
                if (room.CanHaveNeighbour(1) && westNeighbour.CanHaveNeighbour(3))
                    room.SetNeighbour(Directions.west, westNeighbour);
            }
            if (!room.HasNeighbour(2) && generatedRooms.TryGetValue(center + gridStep * new Vector3(0, 0, -1) + room.GetRoomType().southHeightOffset * Vector3.up, out southNeighbour))
            {
                if (room.CanHaveNeighbour(2) && southNeighbour.CanHaveNeighbour(0))
                    room.SetNeighbour(Directions.south, southNeighbour);
            }
            if (!room.HasNeighbour(3) && generatedRooms.TryGetValue(center + gridStep * new Vector3(1, 0, 0) + room.GetRoomType().eastHeightOffset * Vector3.up, out eastNeighbour))
            {
                if (room.CanHaveNeighbour(3) && eastNeighbour.CanHaveNeighbour(1))
                    room.SetNeighbour(Directions.east, eastNeighbour);
            }
        }
    }

    private void UpdateCandidates()
    {
        List<RoomObject> removalList = new();
        foreach(RoomObject room in extensionCandidates)
        {
            if (room.GetNeighboursCount() == room.GetRoomType().maxNeighbours) removalList.Add(room);
        }

        foreach(RoomObject removed in removalList)
        {
            extensionCandidates.Remove(removed);
        }
    }

    private void CreateCoridors()
    {
        // generate coridors only on north and east directions for avoiding collisions
        foreach (RoomObject room in generatedRooms.Values)
        {
            Vector3 roomCenter = room.GetCenter();
            float offsetX = room.GetRoomType().width / 2f;
            float offsetZ = room.GetRoomType().length / 2f;
            RoomObject northNeighbour = room.GetNeighbour(0);
            RoomObject eastNeighbour = room.GetNeighbour(3);
            
            //north direction
            if (northNeighbour != null)
            {
                Vector3 curPos = roomCenter + new Vector3(0, room.GetRoomType().northHeightOffset, offsetZ + 0.5f);
                Debug.Log($"{room.gameObject.name} - generating coridors to north at {curPos}");
                while (!northNeighbour.IsPointOccupied(curPos))
                {
                    Instantiate(coridorVertical, curPos, Quaternion.Euler(0, 0, 0), transform);
                    curPos += Vector3.forward;
                }
            }

            //east direction
            if (eastNeighbour != null)
            {
                Vector3 curPos = roomCenter + new Vector3(offsetX + 0.5f, room.GetRoomType().eastHeightOffset, 0);
                Debug.Log($"{room.gameObject.name} - generating coridors to east at {curPos}");
                while (!eastNeighbour.IsPointOccupied(curPos))
                {
                    Instantiate(coridorHorizontal, curPos, Quaternion.Euler(0, 0, 0), transform);
                    curPos += Vector3.right;
                }
            }
        }
    }

    private void UpdateRoomDoors()
    {
        foreach (RoomObject room in generatedRooms.Values)
        {
            room.UpdateDoors();
        }
    }

    private void BakeNavMesh()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
