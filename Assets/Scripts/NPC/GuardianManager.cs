using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GuardianPatrolData
{
    public List<Transform> patrolPoints = new();
    public bool enterPhase3OnPoints = false;
    public bool addDestinationsToPatrolPoints = false;
}
public class GuardianManager : MonoBehaviour
{
    public static GuardianManager Instance = null;
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

                leastBusiedSpawner.AddToSpawnQueue(guardianData);
            }
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
    }

    public void UpdateInteractableState(Interactable interactable, bool newState)
    {
        foreach (var guardian in guardians)
        {
            guardian.UpdateInteractableState(interactable, newState);
        }
    }
}
