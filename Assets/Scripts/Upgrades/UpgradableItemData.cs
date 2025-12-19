using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeCost
{
    public int level;
    public int cost;
}

[Serializable]
public class UpgradableValue
{
    public int level;
    public float value;
}

[Serializable]
public class UpgradableValueData {
    public string name;
    public bool increaseIsPositive = true;
    public List<UpgradableValue> upgradableValueList;
    public float uvShowMultiplier = 1;
    public string uvSuffix = "";
}

[CreateAssetMenu(fileName = "UpgradableItemData", menuName = "Scriptable Objects/Upgradable Item Data")]
public class UpgradableItemData : ScriptableObject
{
    [Header("Min / Max Levels")]
    public int minLevel = 1;
    public int maxLevel = 1;
    [Header("Upgradable Values")]
    public List<UpgradableValueData> upgradableValues = new();

    [Header("Upgrade costs")]
    public List<UpgradeCost> upgradeCosts;
    [Space(20)]
    [Header("Info")]
    public string itemName;
    [TextArea(5, 10)] public string description;
    [TextArea(5, 10)] public string additionalInfo;
}
