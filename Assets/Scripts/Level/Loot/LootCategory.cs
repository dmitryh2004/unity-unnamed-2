using UnityEngine;

[CreateAssetMenu(fileName = "LootCategory", menuName = "Scriptable Objects/LootCategory")]
public class LootCategory : ScriptableObject
{
    public int id;
    public string lootName;
    [TextArea(5, 10)] public string lootDesc;
    public float volume;
    public int cost;
}
