using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    System.Random random = new System.Random();
    [SerializeField] int roomsMin, roomsMax;
    [SerializeField] int gridStep = 9;
    [SerializeField] GameObject exitRoomPrefab;
    [SerializeField] List<GameObject> roomPrefabs = new();
    List<RoomObject> extensionCandidates = new();
    List<RoomObject> generatedRooms = new();
    [SerializeField] GameObject coridorHorizontal, coridorVertical;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Generate();
    }

    void Generate()
    {
        extensionCandidates.RemoveAll(x => true);
        PlaceRooms();
        CreateCoridors();
        UpdateRoomDoors();
    }

    private void PlaceRooms()
    {
        int roomsRequired = random.Next(roomsMin, roomsMax + 1);
        int roomsCreated = 1;
        GameObject exitRoomGO = Instantiate(exitRoomPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform);
        RoomObject exitRoom = exitRoomGO.GetComponent<RoomObject>();

        generatedRooms.Add(exitRoom);
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
                    GameObject createdRoom = Instantiate(selectedPrefab, center, Quaternion.Euler(0, 0, 0), transform);
                    RoomObject createdRoomObject = createdRoom.GetComponent<RoomObject>();

                    extended.SetNeighbour((int)direction, createdRoomObject);
                    createdRoomObject.SetNeighbour((int)DirectionsController.GetOppositeDirection((Directions)direction), extended);

                    generatedRooms.Add(createdRoomObject);
                    extensionCandidates.Add(createdRoomObject);
                }
                
            }

            roomsCreated++;
            UpdateCandidates();

            if (extensionCandidates.Count == 0) break;
        }
    }

    private bool IsEnoughSpace(Vector3 center, RoomScriptable type)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Vector3 coords = new Vector3(
                            center.x - type.length / 2 + i * type.length,
                            center.y + j * type.height,
                            center.z - type.width / 2 + k * type.width
                        );
                    foreach (RoomObject room in generatedRooms)
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
        
    }

    private void UpdateRoomDoors()
    {
        foreach (RoomObject room in generatedRooms)
        {
            room.UpdateDoors();
        }
    }
}
