using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GuardianPatrolData
{
    public List<Transform> patrolPoints = new();
    public bool enterPhase3OnPoints = false;
    public bool addDestinationsToPatrolPoints = false;
    public int AD_patrolPointsCount = 2;
}
public class GuardianManager : MonoBehaviour
{
    public static GuardianManager Instance = null;
    System.Random random = new ();

    [SerializeField] Transform player;
    [SerializeField] List<GuardianController> guardians = new();
    [SerializeField] TimerController guardianSpawnTimerController;

    [Header("Guardians initial spawn")]
    [SerializeField] bool doInitialSpawn = true;
    [SerializeField] List<GuardianSpawnerController> guardianSpawners = new();
    [SerializeField] List<GuardianPatrolData> guardianPatrolData = new ();
    [SerializeField] float guardianSpawnDelay = 120f;
    float guardianSpawnTimer = 0f;
    bool waitForSpawnTimer = true;

    [Header("Adaptive difficulty")]
    [SerializeField] bool spawnAdditionalGuardians = true;
    [SerializeField] bool changePatrolPointsForSpecifiedGuardians = true;
    [SerializeField] bool additionalGuardiansEnterPhase3OnPoints = false;
    [SerializeField] bool additionalGuardiansAddDestinationsToPatrolPoints = false;
    [SerializeField] int additionalGuardiansPatrolPointsCount = 2;

    List<GuardianPatrolData> additionalGuardiansPatrolData = new ();

    [Header("Call Guardians Options")]
    [SerializeField] bool checkDistance = false;
    [SerializeField] float maxAggroDistance = 100f;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
    }

    private void Start()
    {
        foreach (var guardianSpawner in guardianSpawners)
        {
            guardianSpawner.FindAvailableRooms();
        }

        // add additional guardians if needed
        if (spawnAdditionalGuardians)
        {
            if (AdaptiveDifficultyManager.Instance != null)
            {
                int additionalGuardiansCount = (int)(AdaptiveDifficultyManager.Instance.Values.GetParameterValue("AdditionalGuardiansSpawnAttempts", AdaptiveDifficultyManager.Instance.AlertnessDegree) ?? 0);
                int spawnAdditionalGuardiansChance = (int)((AdaptiveDifficultyManager.Instance.Values.GetParameterValue("SpawnAdditionalGuardiansChance", AdaptiveDifficultyManager.Instance.AlertnessDegree) ?? 0) * 100);
                for (int i = 0; i < additionalGuardiansCount; i++)
                {
                    if (random.Next(0, 100) < spawnAdditionalGuardiansChance)
                    {
                        GuardianPatrolData gpd = new GuardianPatrolData
                        {
                            patrolPoints = new (),
                            enterPhase3OnPoints = additionalGuardiansEnterPhase3OnPoints,
                            addDestinationsToPatrolPoints = additionalGuardiansAddDestinationsToPatrolPoints,
                            AD_patrolPointsCount = additionalGuardiansPatrolPointsCount
                        };
                        additionalGuardiansPatrolData.Add(gpd);
                    }
                }
            }
        }

        if (doInitialSpawn)
        {
            float calculatedSpawnDelay = guardianSpawnDelay;
            if (AdaptiveDifficultyManager.Instance != null)
                calculatedSpawnDelay *= (AdaptiveDifficultyManager.Instance.Values.GetParameterValue("SpawnGuardiansTimeMultiplier", AdaptiveDifficultyManager.Instance.AlertnessDegree) ?? 1);

            guardianSpawnTimer = calculatedSpawnDelay;
            guardianSpawnTimerController.SetRemainingTime(calculatedSpawnDelay);
            guardianSpawnTimerController.StartTimer();
        }
        else
        {
            waitForSpawnTimer = false;
            HideGuardianSpawnTimer();
        }
    }

    void HideGuardianSpawnTimer()
    {
        guardianSpawnTimerController.gameObject.SetActive(false);
    }

    void UpdateSpawnTimer()
    {
        guardianSpawnTimer -= Time.deltaTime;
        if (guardianSpawnTimer <= 0f)
        {
            guardianSpawnTimer = 0f;
            waitForSpawnTimer = false;

            HideGuardianSpawnTimer();

            if (guardianSpawners.Count == 0)
            {
                Debug.LogError("Guardian manager: cannot spawn guardians because no guardian spawners specified.");
                return;
            }

            foreach(var guardianData in guardianPatrolData)
            {
                //find the least busied spawner
                GuardianSpawnerController leastBusiedSpawner = guardianSpawners[0];
                foreach (var spawner in guardianSpawners)
                {
                    if (spawner.GetQueueLength() < leastBusiedSpawner.GetQueueLength()) leastBusiedSpawner = spawner;
                }

                int adjustGuardiansPathChance = 0;
                if (changePatrolPointsForSpecifiedGuardians && AdaptiveDifficultyManager.Instance != null)
                {
                    adjustGuardiansPathChance = (int)((AdaptiveDifficultyManager.Instance.Values.GetParameterValue("AdjustGuardiansPathChance", AdaptiveDifficultyManager.Instance.AlertnessDegree) ?? 0) * 100);
                }
                bool adjustPath = random.Next(0, 100) < adjustGuardiansPathChance;

                leastBusiedSpawner.AddToSpawnQueue(guardianData, adjustPath);
            }

            foreach (var guardianData in additionalGuardiansPatrolData)
            {
                //find the least busied spawner
                GuardianSpawnerController leastBusiedSpawner = guardianSpawners[0];
                foreach (var spawner in guardianSpawners)
                {
                    if (spawner.GetQueueLength() < leastBusiedSpawner.GetQueueLength()) leastBusiedSpawner = spawner;
                }

                leastBusiedSpawner.AddToSpawnQueue(guardianData, true);
            }
        }
    }

    public void RemoveRoomFromSpawnerQueues(RoomObject room)
    {
        foreach(GuardianSpawnerController spawner in guardianSpawners)
        {
            spawner.RemoveRoomFromQueue(room);
        }
    }

    private void Update()
    {
        if (waitForSpawnTimer)
        {
            UpdateSpawnTimer();
        }
    }

    public void ExpireSpawnTimer()
    {
        if (waitForSpawnTimer)
            guardianSpawnTimer = 0f;
    }

    public void AddGuardian(GuardianController guardian)
    {
        guardians.Add(guardian);
    }

    public void CallGuardians()
    {
        foreach (var guardian in guardians)
        {
            bool call = !checkDistance || (checkDistance && (Vector3.Distance(player.position, guardian.transform.position) <= maxAggroDistance));
            if (!call) continue;

            if (guardian.gameObject.activeInHierarchy)
                guardian.CallGuardian(player, player.position);
        }
    }

    public void StopGuardians()
    {
        foreach (var guardian in guardians)
        {
            if (guardian.gameObject.activeInHierarchy)
                guardian.SetActive(false);
        }
        foreach (var spawner in guardianSpawners)
        {
            spawner.Stop();
        }
    }

    public void UpdateInteractableState(Interactable interactable, bool newState)
    {
        foreach (var guardian in guardians)
        {
            guardian.UpdateInteractableState(interactable, newState);
        }
    }
}
