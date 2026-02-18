using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianSpawnerController : MonoBehaviour
{
    [SerializeField] GameObject guardianPrefab;
    [SerializeField] GuardianManager guardianManager;
    [SerializeField] DoorController doorController;
    [SerializeField] List<Transform> trackedObjects = new ();
    [SerializeField] Transform levelPatrolPoints;
    [SerializeField] Transform spawnPoint;
    [SerializeField] PlayerScannerController playerScannerController;

    Queue<GuardianPatrolData> spawnQueue = new();
    bool isSpawning = false;
    [SerializeField] float spawnQueueInterval = 1f, spawnDelay = 2f;

    public IEnumerator SpawnGuardian(GuardianPatrolData guardianData)
    {
        isSpawning = true;

        GuardianController gc = Instantiate(guardianPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0), guardianManager.transform).GetComponent<GuardianController>();
        gc.SetPossibleTargetObjects(trackedObjects);
        gc.SetPatrolPoints(guardianData.patrolPoints);
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

        isSpawning = false;
    }

    public void AddToSpawnQueue(GuardianPatrolData guardianData)
    {
        spawnQueue.Enqueue(guardianData);
    }

    public int GetQueueLength() => spawnQueue.Count;

    void CheckQueue()
    {
        if (isSpawning) return;
        if (spawnQueue.Count > 0)
        {
            StartCoroutine(SpawnGuardian(spawnQueue.Dequeue()));
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckQueue), 0f, spawnQueueInterval);
    }
}
