using UnityEngine;

public class StatisticCollector : MonoBehaviour
{
    public static StatisticCollector Instance = null;
    int totalLootCost = 1;
    int collectedLootCost = 0;
    float percent = 0f;
    int locksHacked = 0;
    int failedHacks = 0;
    int lockedLocks = 0;
    bool alarmRaised = false;
    float alarmRemainedSeconds = 0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void RecalculatePercent()
    {
        percent = (float)collectedLootCost / totalLootCost * 100;
    }

    public int TotalLootCost
    {
        get { return totalLootCost; }
        set { totalLootCost = value; RecalculatePercent(); }
    }

    public int CollectedLootCost
    {
        get { return collectedLootCost; }
        set { collectedLootCost = value; RecalculatePercent(); }
    }

    public float Percent
    {
        get { return percent; }
    }

    public int LocksHacked
    {
        get { return locksHacked; }
        set { locksHacked = value; }
    }

    public int FailedHacks
    {
        get { return failedHacks; }
        set { failedHacks = value; }
    }

    public int LockedLocks
    {
        get { return lockedLocks; }
        set { lockedLocks = value; }
    }

    public bool AlarmRaised
    {
        get { return alarmRaised; }
        set { alarmRaised = value; }
    }

    public float AlarmRemainedSeconds
    {
        get { return alarmRemainedSeconds; }
        set { alarmRemainedSeconds = value; }
    }
}
