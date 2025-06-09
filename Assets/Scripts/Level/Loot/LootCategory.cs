using UnityEngine;

[CreateAssetMenu(fileName = "LootCategory", menuName = "Scriptable Objects/LootCategory")]
public class LootCategory : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public string lootName;
    [TextArea(5, 10)] public string lootDesc;
    public float volume;
    public int cost;

    public GameObject lootPrefab;

    public Vector3 dropRotation = new(-90f, 0f, 0f);
}
