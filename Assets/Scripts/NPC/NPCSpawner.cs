using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner Instance = null;
    [SerializeField] bool doInitialSpawn = true;
    [SerializeField] bool spawnGuardians = true;
    [SerializeField] bool enableDrones = false;
    [SerializeField] float initialSpawnDelay = 600f;
    float initialSpawnTimer = 600f;
    [SerializeField] TimerController spawnTimerController;

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
            float calculatedSpawnDelay = initialSpawnDelay;
            if (AdaptiveDifficultyManager.Instance != null)
                calculatedSpawnDelay *= (AdaptiveDifficultyManager.Instance.Values.GetParameterValue("SpawnGuardiansTimeMultiplier", AdaptiveDifficultyManager.Instance.AlertnessDegree()) ?? 1);

            initialSpawnTimer = calculatedSpawnDelay;
            Debug.Log(initialSpawnTimer);
            spawnTimerController.SetRemainingTime(initialSpawnTimer);
            spawnTimerController.StartTimer();
        }
    }

    private void Update()
    {
        if (doInitialSpawn && initialSpawnTimer >= 0f)
        {
            initialSpawnTimer -= Time.deltaTime;
            if (initialSpawnTimer < 0f)
            {
                initialSpawnTimer = -1f;
                spawnTimerController.gameObject.SetActive(false);
                if (spawnGuardians) GuardianManager.Instance.SpawnGuardians();
                if (enableDrones) DroneManager.Instance.EnableDrones();
            }
        }
    }

    public void ExpireSpawnTimer()
    {
        initialSpawnTimer = 0f;
    }
}
