using System.Collections.Generic;
using UnityEngine;

public class AchievementMessagePanelController : MonoBehaviour
{
    public static AchievementMessagePanelController Instance = null;
    [SerializeField] List<AchievementMessageController> messageControllers = new();
    List<Queue<Achievement>> achievementQueueList = new();
    [SerializeField] float updateRate = 0.5f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        foreach(var mc in messageControllers)
        {
            achievementQueueList.Add(new Queue<Achievement>());
        }
        InvokeRepeating(nameof(CheckMessageControllers), 0f, updateRate);
    }

    void CheckMessageControllers()
    {
        for (int i = 0; i < messageControllers.Count; i++)
        {
            if (!messageControllers[i].IsPlaying && achievementQueueList[i].Count > 0)
            {
                messageControllers[i].ShowAchievement(achievementQueueList[i].Dequeue());
            }
        }
    }

    public void AddAchievement(Achievement ach)
    {
        int leastBusiedQueueIndex = 0;
        int leastBusiedQueueCount = achievementQueueList[0].Count;

        for (int i = 1; i < achievementQueueList.Count; i++)
        {
            if (achievementQueueList[i].Count < leastBusiedQueueCount)
            {
                leastBusiedQueueIndex = i;
                leastBusiedQueueCount = achievementQueueList[leastBusiedQueueIndex].Count;
            }
        }

        achievementQueueList[leastBusiedQueueIndex].Enqueue(ach);
    }
}
