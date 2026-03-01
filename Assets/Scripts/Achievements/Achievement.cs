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
    [TextArea(5, 7)] public string desc;
    public Sprite image;
    public bool hasProgressBar = false;
    public bool hasGoalValue = false;
    public int targetValue = 1;
}
