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

    Queue<List<Transform>> spawnQueue = new();
    bool isSpawning = false;
    [SerializeField] float spawnQueueInterval = 1f, spawnDelay = 2f;

    public IEnumerator SpawnGuardian(List<Transform> patrolPoints)
    {
        isSpawning = true;

        GuardianController gc = Instantiate(guardianPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0), guardianManager.transform).GetComponent<GuardianController>();
        gc.SetTrackedObjects(trackedObjects);
        gc.SetPatrolPoints(patrolPoints);
        playerScannerController.AddHideable(gc.FovLight.gameObject);

        doorController.ChangeDoorState(true);

        yield return new WaitForSeconds(doorController.OpenDoorDuration);

        gc.Init();

        yield return new WaitForSeconds(spawnDelay);

        doorController.ChangeDoorState(false);

        yield return new WaitForSeconds(doorController.CloseDoorDuration);

        isSpawning = false;
    }

    public void AddToSpawnQueue(List<Transform> patrolPoints)
    {
        spawnQueue.Enqueue(patrolPoints);
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
        InvokeRepeating(nameof(CheckQueue), 0f, 1f);
    }
}
