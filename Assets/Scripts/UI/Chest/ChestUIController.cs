using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestUIController : MonoBehaviour
{
    Dictionary<int, int> items, chestItems;
    [SerializeField] Sprite unknownSprite;
    [Header("Inventory")]
    [SerializeField] List<InventoryItem> inventoryItems = new();
    int activeItemID = -1;
    int offset = 0;
    [SerializeField] TMP_Text totalVolume;
    [SerializeField] ProgressBar volumePB;
    [SerializeField] TMP_Text estimateCost;
    [Space(10)]
    [Header("Chest")]
    [SerializeField] List<InventoryItem> chestInventoryItems = new();
    int activeChestItemID = -1;
    int chestOffset = 0;
    [SerializeField] TMP_Text chestEstimateCost;

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

    public void UpdateActiveChestItem()
    {
        foreach (InventoryItem item in chestInventoryItems)
        {
            if (item.IsPointerOnItem())
            {
                activeChestItemID = item.GetID();
                break;
            }
        }
    }

    public void UpdateInventory()
    {
        items = InventorySystem.Instance.GetItems();

        int inventoryItemIndex = inventoryItems.Count - 1 + offset;
        foreach (int i in items.Keys)
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

        UpdateActiveItem();

        float currentVolume = InventorySystem.Instance.GetOccupiedVolume();
        float maxVolume = InventorySystem.Instance.GetMaxVolume();

        float ratio = currentVolume / maxVolume * 100;
        string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

        totalVolume.text = $"{NumberFormatter.FormatNumber(currentVolume * 1000)} / {NumberFormatter.FormatNumber(maxVolume * 1000)} л ({ratio.ToString(format)}%)";
        volumePB.SetMaxValue(maxVolume);
        volumePB.SetProgress(currentVolume);
        estimateCost.text = $"ќценочна€ стоимость вещей: {NumberFormatter.FormatNumberWithGrouping(InventorySystem.Instance.GetTotalCost())} руб.";
    }

    public void UpdateChest()
    {
        chestItems = Chest.Instance.GetItems();

        int chestItemIndex = chestInventoryItems.Count - 1 + offset;
        foreach (int i in chestItems.Keys)
        {
            LootCategory lc = Chest.Instance.GetLootCategoryById(i);

            if (chestItemIndex < chestInventoryItems.Count)
            {
                chestInventoryItems[chestItemIndex].SetActive(true);

                Sprite itemSprite = lc.sprite;
                if (itemSprite == null) itemSprite = unknownSprite;

                chestInventoryItems[chestItemIndex].Initialize(i, itemSprite, chestItems[i]);
                chestInventoryItems[chestItemIndex].UpdateTooltip(lc);
            }

            chestItemIndex--;
        }

        for (; chestItemIndex >= 0; chestItemIndex--)
        {
            chestInventoryItems[chestItemIndex].SetActive(false);
        }

        UpdateActiveChestItem();

        chestEstimateCost.text = $"ќценочна€ стоимость вещей: {NumberFormatter.FormatNumberWithGrouping(Chest.Instance.GetTotalCost())} руб.";
    }

    public void SetActiveItem(int id)
    {
        activeItemID = id;
    }

    public void SetActiveChestItem(int id)
    {
        activeChestItemID = id;
    }

    public void TransferActiveItem()
    {
        if (activeItemID == -1 && activeChestItemID == -1)
        {
            Debug.Log($"Transfer item: no active item");
            return;
        }

        if (activeItemID != -1)
        {
            if (items[activeItemID] <= 0)
            {
                Debug.LogWarning($"Error while transfering: active item count ({items[activeItemID]}) <= 0");
                return;
            }

            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(activeItemID);

            InventorySystem.Instance.RemoveItem(lc);
            Chest.Instance.AddItem(lc);
        }
        else if (activeChestItemID != -1)
        {
            if (chestItems[activeChestItemID] <= 0)
            {
                Debug.LogWarning($"Error while transfering: active item count ({chestItems[activeChestItemID]}) <= 0");
                return;
            }

            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(activeChestItemID);

            if (InventorySystem.Instance.CanAddItem(lc))
            {
                Chest.Instance.RemoveItem(lc);
                InventorySystem.Instance.AddItem(lc);
            }
            else
            {
                Debug.Log($"Unable to transfer item: not enough space in inventory");
            }
        }

        UpdateInventory();
        UpdateChest();
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
            ModifyOffset(3);
        }
    }

    public void ScrollUp()
    {
        if (offset > 0)
            ModifyOffset(-3);
    }

    public void ClearOffset()
    {
        offset = 0;
    }

    void ModifyChestOffset(int diff)
    {
        chestOffset += diff;
        if (chestOffset < 0) chestOffset = 0;
        UpdateChest();
    }

    public void ChestScrollDown()
    {
        if (chestItems.Count - offset > chestInventoryItems.Count)
        {
            ModifyChestOffset(3);
        }
    }

    public void ChestScrollUp()
    {
        if (chestOffset > 0)
            ModifyChestOffset(-3);
    }

    public void ClearChestOffset()
    {
        chestOffset = 0;
    }
}
