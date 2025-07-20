using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootCategoryManager", menuName = "Scriptable Objects/LootCategoryManager")]
public class LootCategoryManager : ScriptableObject
{
    public List<LootCategory> lootCategories = new();
}
