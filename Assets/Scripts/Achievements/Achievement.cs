using UnityEngine;

public enum AchievementCategory
{
    Other = -1,
    Flies = 1,
    Quota = 2,
    Items = 3,
    Locations = 4,
    Equipment = 5
}

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class Achievement : ScriptableObject
{
    public AchievementCategory categoryID = AchievementCategory.Other;
    public string id;
    public string title;
    public string desc;
    public Sprite image;
    public bool hasProgressBar = false;
    public int progressBarValue = 0;
    public bool hasGoalValue = false;
    public int goalValue = 0;
}
