using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance = null;
    [SerializeField] AchievementManager achievementManager;

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

    public bool IsAchievementAchieved(string id)
    {
        Achievement ach = GetAchievementByID(id);
        if (ach != null)
        {
            int progress = GetAchievementProgress(id);
            return progress >= ach.targetValue;
        }
        else return false;
    }

    public int GetAchievementCount() => achievementManager.GetAchievementCount();

    public Achievement GetAchievementByID(string id)
    {
        return achievementManager.GetAchievementByID(id);
    }

    public Achievement GetAchievementByIndex(int index)
    {
        return achievementManager.GetAchievementByIndex(index);
    }

    public void SetAchievementProgress(string id, int progress)
    {
        Achievement ach = GetAchievementByID(id);
        if (ach != null)
        {
            PlayerPrefs.SetInt($"Achievement_{ach.id}_Progress", progress);
            PlayerPrefs.Save();
            print($"achievement {ach.title} progress saved");
        }
    }

    public void SetAchievementProgress(int index, int progress)
    {
        Achievement ach = GetAchievementByIndex(index);
        if (ach != null)
        {
            PlayerPrefs.SetInt($"Achievement_{ach.id}_Progress", progress);
            PlayerPrefs.Save();
        }
    }

    public int GetAchievementProgress(string id)
    {
        Achievement ach = GetAchievementByID(id);
        if (ach != null)
        {
            return PlayerPrefs.GetInt($"Achievement_{ach.id}_Progress", 0);
        }
        else return -1;
    }

    public int GetAchievementProgress(int index)
    {
        Achievement ach = GetAchievementByIndex(index);
        if (ach != null)
        {
            return PlayerPrefs.GetInt($"Achievement_{ach.id}_Progress", 0);
        }
        else return -1;
    }

    void ModifyAchievementProgress(Achievement ach, bool isGoalValue, int diff)
    {
        if (ach != null)
        {
            int currentProgress = GetAchievementProgress(ach.id);
            if (isGoalValue == true) // goal value
            {
                SetAchievementProgress(ach.id, Mathf.Max(currentProgress, diff));
            }
            else // standard
            {
                SetAchievementProgress(ach.id, currentProgress + diff);
            }
        }
    }

    public void ModifyAchievementProgress(string id, bool isGoalValue, int diff)
    {
        Achievement ach = GetAchievementByID(id);
        if (ach != null)
        {
            ModifyAchievementProgress(ach, isGoalValue, diff);
        }
    }

    public void ModifyAchievementProgress(int index, bool isGoalValue, int diff)
    {
        Achievement ach = GetAchievementByIndex(index);
        if (ach != null)
        {
            ModifyAchievementProgress(ach, isGoalValue, diff);
        }
    }
}
