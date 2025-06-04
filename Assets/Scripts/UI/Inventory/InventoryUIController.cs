using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    Dictionary<int, int> items;
    [SerializeField] List<InventoryItem> inventoryItems = new();
    int activeItemID = -1;
    [SerializeField] TMP_Text totalVolume;
    [SerializeField] ProgressBar volumePB;
    [SerializeField] TMP_Text estimateCost;
    [SerializeField] Sprite unknownSprite;

    public void UpdateActiveItem()
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.IsPointerOnItem())
            {
                activeItemID = item.GetID();
                break;
            }
        }
    }

    public void UpdateInventory()
    {
        items = InventorySystem.Instance.GetItems();

        float totalCost = 0f;

        int inventoryItemIndex = 27;
        foreach(int i in items.Keys)
        {
            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(i);
            Debug.Log($"{i} ({lc.lootName}): стоимость 1 шт.={lc.cost}");
            totalCost += items[i] * lc.cost;

            if (inventoryItemIndex < inventoryItems.Count)
            {
                inventoryItems[inventoryItemIndex].gameObject.SetActive(true);

                Sprite itemSprite = lc.sprite;
                if (itemSprite == null) itemSprite = unknownSprite;
                Debug.Log($"{i}: {itemSprite}");

                inventoryItems[inventoryItemIndex].Initialize(i, itemSprite, items[i]);
                inventoryItems[inventoryItemIndex].UpdateTooltip(lc);
            }

            inventoryItemIndex--;
        }

        for (; inventoryItemIndex >= 0; inventoryItemIndex--)
        {
            inventoryItems[inventoryItemIndex].gameObject.SetActive(false);
        }

        float currentVolume = InventorySystem.Instance.GetOccupiedVolume();
        float maxVolume = InventorySystem.Instance.maxVolume;

        float ratio = currentVolume / maxVolume * 100;
        string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

        totalVolume.text = $"{currentVolume} / {maxVolume} м3 ({(currentVolume / maxVolume * 100).ToString(format)}%)";
        volumePB.SetProgress(currentVolume);
        estimateCost.text = $"ќценочна€ стоимость вещей: {totalCost} руб.";
    }

    public void SetActiveItem(int id)
    {
        activeItemID = id;
    }

    public void DropActiveItem(Transform spawnPosition)
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

        InventorySystem.Instance.RemoveItem(lc);
        GameObject.Instantiate(lc.lootPrefab, spawnPosition.position, Quaternion.Euler(-90f, 0f, 0f));

        UpdateInventory();
        UpdateActiveItem();
    }
}
