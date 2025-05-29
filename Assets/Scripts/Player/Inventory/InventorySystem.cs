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

    [Tooltip("������������ ����� ���������")]
    public float maxVolume = 0.01f;

    [Tooltip("������ ���� ��������� ����� ��������� (ScriptableObject)")]
    public List<LootCategory> lootCategories;

    // �������: ���� - id LootCategory, �������� - ���������� ��������� ������� ����
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
    /// �������� ��������� ������� ����� � ���������
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
    /// ���������, ����� �� �������� ������� ��������� ���� � ���������
    /// </summary>
    /// <param name="lootCategory">��� ��������</param>
    /// <returns>True, ���� �������� �����, ����� false</returns>
    public bool CanAddItem(LootCategory lootCategory)
    {
        if (lootCategory == null)
            return false;

        float newVolume = GetOccupiedVolume() + lootCategory.volume;
        return newVolume <= maxVolume;
    }

    /// <summary>
    /// �������� ������� ��������� ���� � ���������
    /// </summary>
    /// <param name="lootCategory">��� ��������</param>
    /// <returns>True, ���� ������� ������� ��������, ����� false</returns>
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
    /// ������� ���� ������� ��������� ���� �� ���������
    /// </summary>
    /// <param name="lootCategory">��� ��������</param>
    /// <returns>True, ���� ������� ������� �����, ����� false</returns>
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
    /// �������� ������ ���� ��������� � ��������� � JSON-�������
    /// </summary>
    /// <returns>JSON ������ � ������� ���������</returns>
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
    /// �������� LootCategory �� id
    /// </summary>
    private LootCategory GetLootCategoryById(int id)
    {
        return lootCategories.FirstOrDefault(x => x.id == id);
    }

    // ��������������� ����� ��� ������������ ������ � JSON
    [Serializable]
    private class InventoryDataWrapper
    {
        public List<InventoryItemData> items;
    }
}
