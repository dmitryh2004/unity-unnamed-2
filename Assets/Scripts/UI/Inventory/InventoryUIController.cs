using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    Dictionary<int, int> items;
    [SerializeField] List<InventoryItem> inventoryItems = new();
    int activeItemID = -1;
    int offset = 0;
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

        int inventoryItemIndex = inventoryItems.Count - 1 + offset;
        foreach(int i in items.Keys)
        {
            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(i);

            if (inventoryItemIndex < inventoryItems.Count)
            {
                inventoryItems[inventoryItemIndex].SetActive(true);

                Sprite itemSprite = lc.sprite;
                if (itemSprite == null) itemSprite = unknownSprite;

                inventoryItems[inventoryItemIndex].Initialize(i, itemSprite, items[i]);
                inventoryItems[inventoryItemIndex].UpdateTooltip(lc);
            }

            inventoryItemIndex--;
        }

        for (; inventoryItemIndex >= 0; inventoryItemIndex--)
        {
            inventoryItems[inventoryItemIndex].SetActive(false);
        }

        float currentVolume = InventorySystem.Instance.GetOccupiedVolume();
        float maxVolume = InventorySystem.Instance.GetMaxVolume();

        float ratio = currentVolume / maxVolume * 100;
        string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

        totalVolume.text = $"{NumberFormatter.FormatNumber(currentVolume * 1000)} / {NumberFormatter.FormatNumber(maxVolume * 1000)} л ({ratio.ToString(format)}%)";
        volumePB.SetMaxValue(maxVolume);
        volumePB.SetProgress(currentVolume);
        estimateCost.text = $"Оценочная стоимость вещей: {NumberFormatter.FormatNumberWithGrouping(InventorySystem.Instance.GetTotalCost())} руб.";
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
        GameObject.Instantiate(lc.lootPrefab, spawnPosition.position, Quaternion.Euler(lc.dropRotation));

        UpdateInventory();
        UpdateActiveItem();
    }

    void ModifyOffset(int diff)
    {
        offset += diff;
        if (offset < 0) offset = 0;
        UpdateInventory();
    }

    public void ScrollDown()
    {
        if (items.Count - offset > inventoryItems.Count)
        {
            ModifyOffset(7);
        }
    }

    public void ScrollUp()
    {
        if (offset > 0)
            ModifyOffset(-7);
    }

    public void ClearOffset()
    {
        offset = 0;
    }
}
