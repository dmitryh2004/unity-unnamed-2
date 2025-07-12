using UnityEngine;

[CreateAssetMenu(fileName = "Complex", menuName = "Scriptable Objects/Complex")]
public class Complex : ScriptableObject
{
    public string complexName;
    [TextArea(5, 10)]
    public string description;
    [Range(1, 10)]
    public int difficulty;
    public int minRooms;
    public int maxRooms;
    public int guardiansCount;
    public string reinforcementTimer;
}
