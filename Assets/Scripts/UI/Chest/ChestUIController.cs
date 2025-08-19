using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestUIController : MonoBehaviour
{
    Dictionary<int, int> items, chestItems;
    [SerializeField] Sprite unknownSprite;
    [Header("Inventory")]
    [SerializeField] InventoryLayoutElement inventoryLayoutElement;
    int activeItemID = -1;
    [Space(10)]
    [Header("Chest")]
    [SerializeField] InventoryLayoutElement chestLayoutElement;
    int activeChestItemID = -1;

    string GetItemsString()
    {
        string res = "";
        foreach (var kvp in items)
        {
            if (res != "") res += "; ";
            res += $"{kvp.Key} => {kvp.Value}";
        }
        return res;
    }

    string GetChestItemsString()
    {
        string res = "";
        foreach (var kvp in chestItems)
        {
            if (res != "") res += "; ";
            res += $"{kvp.Key} => {kvp.Value}";
        }
        return res;
    }

    public void UpdateActiveItem()
    {
        inventoryLayoutElement.UpdateActiveItem();
        activeItemID = inventoryLayoutElement.GetActiveItemID();
    }

    public void UpdateActiveChestItem()
    {
        chestLayoutElement.UpdateActiveItem();
        activeChestItemID = chestLayoutElement.GetActiveItemID();
    }

    public void UpdateInventory()
    {
        items = InventorySystem.Instance.GetItems();

        inventoryLayoutElement.UpdateLayout(items);
        UpdateActiveItem();
    }

    public void UpdateChest()
    {
        chestItems = Chest.Instance.GetItems();

        chestLayoutElement.UpdateLayout(chestItems);
        UpdateActiveChestItem();
    }

    public void SetActiveItem(int id)
    {
        activeItemID = id;
        inventoryLayoutElement.SetActiveItemID(id);
    }

    public void SetActiveChestItem(int id)
    {
        activeChestItemID = id;
        chestLayoutElement.SetActiveItemID(id);
    }

    public void TransferActiveItem(bool all = false)
    {
        if (activeItemID == -1 && activeChestItemID == -1)
        {
            Debug.Log($"Transfer item: no active item");
            return;
        }

        if (activeItemID != -1)
        {
            if (!items.ContainsKey(activeItemID))
            {
                Debug.LogWarning($"Error while transfering: active item ({activeItemID}) does not exist. Items: {GetItemsString()}");
                return;
            }
            if (items[activeItemID] <= 0)
            {
                Debug.LogWarning($"Error while transfering: active item count ({items[activeItemID]}) <= 0. Items: {GetItemsString()}");
                return;
            }

            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(activeItemID);

            if (Chest.Instance.CanAddItem())
            {
                if (all)
                {
                    while (items.ContainsKey(activeItemID) && items[activeItemID] > 0 && Chest.Instance.CanAddItem())
                    {
                        InventorySystem.Instance.RemoveItem(lc);
                        Chest.Instance.AddItem(lc);
                    }
                }
                else
                {
                    InventorySystem.Instance.RemoveItem(lc);
                    Chest.Instance.AddItem(lc);
                }
            }
            else
            {
                Debug.Log($"Unable to transfer item: not enough space in chest");
            }
        }
        else if (activeChestItemID != -1)
        {
            if (!chestItems.ContainsKey(activeChestItemID))
            {
                Debug.LogWarning($"Error while transfering: active item ({activeChestItemID}) does not exist. Chest items: {GetChestItemsString()}");
                return;
            }
            if (chestItems[activeChestItemID] <= 0)
            {
                Debug.LogWarning($"Error while transfering: active item count ({chestItems[activeChestItemID]}) <= 0. Chest items: {GetChestItemsString()}");
                return;
            }

            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(activeChestItemID);

            if (InventorySystem.Instance.CanAddItem(lc))
            {
                if (all)
                {
                    while (chestItems.ContainsKey(activeChestItemID) && chestItems[activeChestItemID] > 0 && InventorySystem.Instance.CanAddItem(lc))
                    {
                        Chest.Instance.RemoveItem(lc);
                        InventorySystem.Instance.AddItem(lc);
                    }
                }
                else
                {
                    Chest.Instance.RemoveItem(lc);
                    InventorySystem.Instance.AddItem(lc);
                }
            }
            else
            {
                Debug.Log($"Unable to transfer item: not enough space in inventory");
            }
        }

        UpdateInventory();
        UpdateChest();
    }

    public void ScrollDown()
    {
        inventoryLayoutElement.ScrollDown();
    }

    public void ScrollUp()
    {
        inventoryLayoutElement.ScrollUp();
    }

    public void ClearOffset()
    {
        inventoryLayoutElement.ClearOffset();
        UpdateInventory();
    }

    public void ChestScrollDown()
    {
        chestLayoutElement.ScrollDown();
    }

    public void ChestScrollUp()
    {
        chestLayoutElement.ScrollUp();
    }

    public void ClearChestOffset()
    {
        chestLayoutElement.ClearOffset();
        UpdateChest();
    }
}
