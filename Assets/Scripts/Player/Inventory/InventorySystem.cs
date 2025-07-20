using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class InventoryItemData
{
    public int lootCategoryId;
    public int quantity;

    public InventoryItemData(int lootCategoryId, int quantity)
    {
        this.lootCategoryId = lootCategoryId;
        this.quantity = quantity;
    }
}

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance = null;

    [SerializeField]
    [Range(1, 4)] int level = 1;

    // Максимальный объем инвентаря
    float maxVolume = 0.01f;

    public LootCategoryManager lootCategoryManager;
    //Список всех доступных типов предметов (ScriptableObject)
    List<LootCategory> lootCategories;

    // Словарь: ключ - id LootCategory, значение - количество предметов данного типа
    private Dictionary<int, int> items = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        lootCategories = lootCategoryManager.lootCategories;
    }

    private void Start()
    {
        RecalculateMaxSize();
    }

    public float GetMaxVolume() => maxVolume;
    public int GetLevel() => level;
    public void SetLevel(int level)
    {
        this.level = level;
        RecalculateMaxSize();
    }

    void RecalculateMaxSize()
    {
        switch (level)
        {
            case 1:
                maxVolume = 0.01f;
                break;
            case 2:
                maxVolume = 0.02f;
                break;
            case 3:
                maxVolume = 0.04f;
                break;
            case 4:
                maxVolume = 0.08f;
                break;
            default:
                maxVolume = 0.01f;
                break;
        }
    }

    /// <summary>
    /// Получить суммарный занятый объем в инвентаре
    /// </summary>
    public float GetOccupiedVolume()
    {
        float totalVolume = 0f;
        foreach (var kvp in items)
        {
            LootCategory category = GetLootCategoryById(kvp.Key);
            if (category != null)
            {
                totalVolume += category.volume * kvp.Value;
            }
        }
        return totalVolume;
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

    /// <summary>
    /// Проверить, можно ли добавить предмет заданного типа в инвентарь
    /// </summary>
    /// <param name="lootCategory">Тип предмета</param>
    /// <returns>True, если добавить можно, иначе false</returns>
    public bool CanAddItem(LootCategory lootCategory, int amount = 1)
    {
        if (lootCategory == null)
            return false;

        float currentVolume = MathF.Round(GetOccupiedVolume(), 6);
        float itemVolume = MathF.Round(lootCategory.volume, 6);
        float newVolume = MathF.Round(currentVolume + itemVolume * amount, 6);
        float roundedMaxVolume = MathF.Round(maxVolume, 6);

        Debug.Log($"volume: current - {currentVolume}, diff - {itemVolume}, new - {newVolume} (max = {roundedMaxVolume})");
        return newVolume <= roundedMaxVolume;
    }

    /// <summary>
    /// Добавить предмет заданного типа в инвентарь
    /// </summary>
    /// <param name="lootCategory">Тип предмета</param>
    /// <returns>True, если предмет успешно добавлен, иначе false</returns>
    public bool AddItem(LootCategory lootCategory)
    {
        if (lootCategory == null)
            return false;

        if (!CanAddItem(lootCategory))
            return false;

        if (items.ContainsKey(lootCategory.id))
        {
            items[lootCategory.id]++;
        }
        else
        {
            items.Add(lootCategory.id, 1);
        }
        return true;
    }

    /// <summary>
    /// Удалить один предмет заданного типа из инвентаря
    /// </summary>
    /// <param name="lootCategory">Тип предмета</param>
    /// <returns>True, если предмет успешно удалён, иначе false</returns>
    public bool RemoveItem(LootCategory lootCategory, bool all = false)
    {
        if (lootCategory == null)
            return false;

        if (!items.ContainsKey(lootCategory.id))
            return false;

        if (all)
        {
            items.Remove(lootCategory.id);
        }
        else
        {
            items[lootCategory.id]--;

            if (items[lootCategory.id] <= 0)
            {
                items.Remove(lootCategory.id);
            }
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
                    if (CanAddItem(lc))
                    {
                        AddItem(lc);
                    }
                }
            }
        }
    }
}
