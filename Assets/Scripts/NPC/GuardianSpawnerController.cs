using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomObjectWeightComparer : IComparer<RoomObject>
{
    public int Compare(RoomObject x, RoomObject y)
    {
        float diff = x.RoomWeight - y.RoomWeight;
        return (int)Mathf.Sign(diff);
    }
}

public class GuardianSpawnerController : MonoBehaviour
{
    [SerializeField] GameObject guardianPrefab;
    [SerializeField] Transform patrolPointParent;
    [SerializeField] GameObject patrolPointPrefab;
    [SerializeField] GuardianManager guardianManager;
    [SerializeField] DoorController doorController;
    [SerializeField] List<Transform> trackedObjects = new ();
    //[SerializeField] Transform levelPatrolPoints;
    [SerializeField] Transform spawnPoint;
    [SerializeField] PlayerScannerController playerScannerController;
    [SerializeField] RoomObject roomObject;
    Queue<RoomObject> availableRooms = new ();

    Queue<KeyValuePair<GuardianPatrolData, bool>> spawnQueue = new();
    bool isSpawning = false;
    [SerializeField] float spawnQueueInterval = 1f, spawnDelay = 2f;

    public void FindAvailableRooms()
    {
        List<RoomObject> availableRoomsList = roomObject.GetAvailableRooms();
        availableRoomsList.Sort(new RoomObjectWeightComparer());
        availableRoomsList.Reverse();
        availableRooms = new Queue<RoomObject>(availableRoomsList);
    }
    public void RemoveRoomFromQueue(RoomObject room)
    {
        if (!availableRooms.Contains(room)) return;

        Queue<RoomObject> newQueue = new Queue<RoomObject>();
        while (availableRooms.Count > 0)
        {
            RoomObject temp = availableRooms.Dequeue();
            if (temp != room) newQueue.Enqueue(temp);
        }
        availableRooms = newQueue;
    }
    public IEnumerator SpawnGuardian(GuardianPatrolData guardianData, bool changePatrolPoints = false)
    {
        isSpawning = true;

        if (availableRooms.Count < guardianData.AD_patrolPointsCount)
        {
            Debug.LogWarning($"Unable to find {guardianData.AD_patrolPointsCount} in {availableRooms.Count} available rooms. Maybe guardian count is too big for location?");
        }
        else
        {
            GuardianController gc = Instantiate(guardianPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0), guardianManager.transform).GetComponent<GuardianController>();
            gc.SetPossibleTargetObjects(trackedObjects);

            if (changePatrolPoints)
            {
                List<Transform> patrolPoints = new ();
                for (int i = 0; i < guardianData.AD_patrolPointsCount; i++)
                {
                    RoomObject room = availableRooms.Dequeue();
                    guardianManager.RemoveRoomFromSpawnerQueues(room);
                    Transform patrolPoint = GameObject.Instantiate(patrolPointPrefab, room.GetCenter(), Quaternion.Euler(0, 0, 0), patrolPointParent).transform;
                    patrolPoints.Add(patrolPoint);
                }
                gc.SetPatrolPoints(patrolPoints);
            }
            else
            {
                gc.SetPatrolPoints(guardianData.patrolPoints);
            }

            gc.SetAddDestinationsToPatrolPoints(guardianData.addDestinationsToPatrolPoints);
            gc.SetEnterPhase3OnPoints(guardianData.enterPhase3OnPoints);
            guardianManager.AddGuardian(gc);

            playerScannerController.AddHideable(gc.FovLight.gameObject);

            doorController.ChangeDoorState(true);

            yield return new WaitForSeconds(doorController.OpenDoorDuration);

            gc.Init();

            yield return new WaitForSeconds(spawnDelay);

            doorController.ChangeDoorState(false);

            yield return new WaitForSeconds(doorController.CloseDoorDuration);
        }

        isSpawning = false;
    }

    public void AddToSpawnQueue(GuardianPatrolData guardianData, bool adjustPath = false)
    {
        spawnQueue.Enqueue(new KeyValuePair<GuardianPatrolData, bool>(guardianData, adjustPath));
    }

    public int GetQueueLength() => spawnQueue.Count;

    void CheckQueue()
    {
        //Debug.Log($"spawning: {isSpawning}");
        if (isSpawning) return;
        if (spawnQueue.Count > 0)
        {
            KeyValuePair<GuardianPatrolData, bool> guardianPatrolData = spawnQueue.Dequeue();
            StartCoroutine(SpawnGuardian(guardianPatrolData.Key, guardianPatrolData.Value));
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckQueue), 0f, spawnQueueInterval);
    }
}
