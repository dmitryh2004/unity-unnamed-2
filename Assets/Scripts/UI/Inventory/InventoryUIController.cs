using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    Dictionary<int, int> items;
    [SerializeField] InventoryLayoutElement inventoryLayoutElement;
    int activeItemID = -1;

    public void UpdateActiveItem()
    {
        inventoryLayoutElement.UpdateActiveItem();
        activeItemID = inventoryLayoutElement.GetActiveItemID();
    }

    public void UpdateInventory()
    {
        items = InventorySystem.Instance.GetItems();

        inventoryLayoutElement.UpdateLayout(items);
    }

    public void SetActiveItem(int id)
    {
        activeItemID = id;
        inventoryLayoutElement.SetActiveItemID(id);
    }

    public void DropActiveItem(Transform spawnPosition, bool all = false)
    {
        if (activeItemID == -1) 
        {
            Debug.Log("Drop item: no active item");
            return; 
        }

        if (items[activeItemID] <= 0)
        {
            Debug.LogWarning($"Found an error while dropping item: active item count ({items[activeItemID]}) <= 0");
            return;
        }

        LootCategory lc = InventorySystem.Instance.GetLootCategoryById(activeItemID);

        if (all)
        {
            int count = items[activeItemID];
            InventorySystem.Instance.RemoveItem(lc, true);
            for (int i = 0; i < count; i++)
            {
                GameObject.Instantiate(lc.lootPrefab, spawnPosition.position, Quaternion.Euler(lc.dropRotation));
            }
        }
        else
        {
            InventorySystem.Instance.RemoveItem(lc);
            GameObject.Instantiate(lc.lootPrefab, spawnPosition.position, Quaternion.Euler(lc.dropRotation));
        }
        UpdateInventory();
        UpdateActiveItem();
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
    }
}
