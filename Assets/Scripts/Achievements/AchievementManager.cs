using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement manager", menuName = "Scriptable Objects/Achievement Manager")]
public class AchievementManager : ScriptableObject
{
    [SerializeField] List<Achievement> achievements = new ();

    public Achievement GetAchievementByID(string id)
    {
        return achievements.Find((x) => x.id == id) ?? null;
    }

    public Achievement GetAchievementByIndex(int index)
    {
        if (index < 0 || index >= achievements.Count) return null;
        return achievements[index];
    }

    public int GetAchievementCount()
    {
        return achievements.Count;
    }
}
