using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootCostModifier
{
    public int itemID;
    public float modifier;
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
            dict[item.itemID] = item.modifier;
        }
        return dict;
    }

    public int days = 4;

    public bool hasEarlyCompletionBonus;
    public float earlyCompletionBonusModifier2; //за 2 дня до дедлайна
    public float earlyCompletionBonusModifier1; //за 1 день до дедлайна
}
