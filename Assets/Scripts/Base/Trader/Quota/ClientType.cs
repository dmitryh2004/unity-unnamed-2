using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootCostModifier
{
    public int key;
    public float value;
}

[CreateAssetMenu(fileName = "ClientType", menuName = "Scriptable Objects/Quota/Client Type")]
public class ClientType : ScriptableObject
{
    public string clientType;
    public List<LootCostModifier> lootCostModifiers;
    
    public Dictionary<int, float> GetLootCostModifiersDictionary()
    {
        var dict = new Dictionary<int, float>();
        foreach (var item in lootCostModifiers)
        {
            dict[item.key] = item.value;
        }
        return dict;
    }

    public bool hasEarlyCompletionBonus;
    public int earlyCompletionBonus2; //за 2 дня до дедлайна
    public int earlyCompletionBonus1; //за 1 день до дедлайна
}
