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
[CreateAssetMenu(fileName = "UpgradableItemData", menuName = "Scriptable Objects/Upgradable Item Data")]
public class UpgradableItemData : ScriptableObject
{
    [Header("Min / Max Levels")]
    public int minLevel = 1;
    public int maxLevel = 1;
    [Header("Upgradable Values")]
    public bool hasUpgradableValue1 = true;
    public List<UpgradableValue> upgradableValue1List;
    [Space()]
    public bool hasUpgradableValue2 = false;
    public List<UpgradableValue> upgradableValue2List;
    [Space()]
    public bool hasUpgradableValue3 = false;
    public List<UpgradableValue> upgradableValue3List;
    [Header("Upgrade costs")]
    public List<UpgradeCost> upgradeCosts;
    [Space(20)]
    [Header("Info")]
    public string itemName;
    [TextArea(5, 10)] public string description;
    public string uv1Name, uv2Name, uv3Name;
    [TextArea(5, 10)] public string additionalInfo;
}
