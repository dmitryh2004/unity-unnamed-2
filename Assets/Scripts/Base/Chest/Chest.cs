using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public static Chest Instance = null;
    int totalItemsAmount = 0;
    int maxItemsAmount = 1000;

    public LootCategoryManager lootCategoryManager;
    // список всех предметов. берется из InventorySystem
    List<LootCategory> lootCategories;

    private Dictionary<int, int> items = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        lootCategories = lootCategoryManager.lootCategories;
    }

    public int GetTotalCost()
    {
        int totalCost = 0;
        foreach (var kvp in items)
        {
            LootCategory category = GetLootCategoryById(kvp.Key);
            if (category != null)
            {
                totalCost += category.cost * kvp.Value;
            }
            if (totalCost >= 2_000_000_000 || totalCost < 0)
            {
                totalCost = 2_000_000_000;
                break;
            }
        }
        return totalCost;
    }

    public int GetTotalItemsAmount()
    {
        return totalItemsAmount;
    }

    public int GetMaxItemsAmount()
    {
        return maxItemsAmount;
    }

    public bool CanAddItem()
    {
        return totalItemsAmount < maxItemsAmount;
    }

    public bool AddItem(LootCategory lootCategory)
    {
        if (lootCategory == null)
            return false;

        if (!CanAddItem())
            return false;

        if (items.ContainsKey(lootCategory.id))
        {
            items[lootCategory.id]++;
        }
        else
        {
            items.Add(lootCategory.id, 1);
        }
        totalItemsAmount++;
        return true;
    }

    public bool RemoveItem(LootCategory lootCategory, bool all = false)
    {
        if (lootCategory == null)
            return false;

        if (!items.ContainsKey(lootCategory.id))
            return false;

        if (all)
        {
            totalItemsAmount -= items[lootCategory.id];
            items.Remove(lootCategory.id);
        }
        else
        {
            items[lootCategory.id]--;

            if (items[lootCategory.id] <= 0)
            {
                items.Remove(lootCategory.id);
            }
            totalItemsAmount--;
        }
        return true;
    }

    public void RemoveAllItems()
    {
        items.Clear();
    }

    /// <summary>
    /// Получить данные всех предметов в инвентаре в JSON-формате
    /// </summary>
    /// <returns>JSON строка с данными предметов</returns>
    public string GetInventoryDataJson()
    {
        List<InventoryItemData> dataList = new List<InventoryItemData>();
        foreach (var kvp in items)
        {
            dataList.Add(new InventoryItemData(kvp.Key, kvp.Value));
        }

        InventoryDataWrapper wrapper = new InventoryDataWrapper() { items = dataList };
        return JsonUtility.ToJson(wrapper, false);
    }

    public Dictionary<int, int> GetItems()
    {
        return items;
    }

    /// <summary>
    /// Получить LootCategory по id
    /// </summary>
    public LootCategory GetLootCategoryById(int id)
    {
        return lootCategories.FirstOrDefault(x => x.id == id);
    }

    // Вспомогательный класс для сериализации списка в JSON
    [Serializable]
    private class InventoryDataWrapper
    {
        public List<InventoryItemData> items;
    }

    public void SetItemsFromJson(string json)
    {
        InventoryDataWrapper wrapper = JsonUtility.FromJson<InventoryDataWrapper>(json);
        if (wrapper != null)
        {
            foreach (InventoryItemData inventoryItem in wrapper.items)
            {
                LootCategory lc = GetLootCategoryById(inventoryItem.lootCategoryId);

                for (int i = 0; i < inventoryItem.quantity; i++)
                {
                    if (CanAddItem())
                        AddItem(lc);
                }
            }
        }
    }
}
