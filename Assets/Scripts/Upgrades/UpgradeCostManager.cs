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
[CreateAssetMenu(fileName = "UpgradeCostManager", menuName = "Scriptable Objects/UpgradeCostManager")]
public class UpgradeCostManager : ScriptableObject
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
}
