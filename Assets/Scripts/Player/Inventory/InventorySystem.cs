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

    [Tooltip("Максимальный объем инвентаря")]
    public float maxVolume = 0.01f;

    [Tooltip("Список всех доступных типов предметов (ScriptableObject)")]
    public List<LootCategory> lootCategories;

    // Словарь: ключ - id LootCategory, значение - количество предметов данного типа
    private Dictionary<int, int> items = new Dictionary<int, int>();

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

    /// <summary>
    /// Проверить, можно ли добавить предмет заданного типа в инвентарь
    /// </summary>
    /// <param name="lootCategory">Тип предмета</param>
    /// <returns>True, если добавить можно, иначе false</returns>
    public bool CanAddItem(LootCategory lootCategory)
    {
        if (lootCategory == null)
            return false;

        float newVolume = GetOccupiedVolume() + lootCategory.volume;
        return newVolume <= maxVolume;
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
    public bool RemoveItem(LootCategory lootCategory)
    {
        if (lootCategory == null)
            return false;

        if (!items.ContainsKey(lootCategory.id))
            return false;

        items[lootCategory.id]--;

        if (items[lootCategory.id] <= 0)
        {
            items.Remove(lootCategory.id);
        }
        return true;
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
        return JsonUtility.ToJson(wrapper, true);
    }

    /// <summary>
    /// Получить LootCategory по id
    /// </summary>
    private LootCategory GetLootCategoryById(int id)
    {
        return lootCategories.FirstOrDefault(x => x.id == id);
    }

    // Вспомогательный класс для сериализации списка в JSON
    [Serializable]
    private class InventoryDataWrapper
    {
        public List<InventoryItemData> items;
    }
}
