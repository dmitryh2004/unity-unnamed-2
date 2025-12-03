using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JewerlyTableOutputVariantRange
{
    public int min, max;
}

[System.Serializable]
public class JewerlyTableOutputVariant
{
    public List<UpgradableValue> weight;
    public List<LootCategory> outputItems;
    public List<JewerlyTableOutputVariantRange> outputItemsAmount;
}

[CreateAssetMenu(fileName = "Jewerly Table Craft", menuName = "Scriptable Objects/JewerlyTable/Craft")]
public class JewerlyTableCraft : ScriptableObject
{
    public string craftName;

    [Header("Input items")]
    public List<LootCategory> requiredItems = new();
    public List<int> requiredItemsCount = new();

    [Header("Output variants")]
    public List<JewerlyTableOutputVariant> outputVariants;
}
