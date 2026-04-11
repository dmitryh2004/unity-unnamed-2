using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryLayoutElement : MonoBehaviour
{
    [SerializeField] LootCategoryManager lootCategoryManager;
    [SerializeField] List<InventoryItem> itemElements = new ();
    [SerializeField] int offsetStep;
    [Space()]
    [SerializeField] bool hasVolume;
    [SerializeField] TMP_Text totalVolume;
    [SerializeField] ProgressBar volumePB;
    [Space()]
    [SerializeField] bool hasCount;
    [SerializeField] TMP_Text totalCount;
    [SerializeField] ProgressBar countPB;
    [Space()]
    [SerializeField] bool hasEstimateCost;
    [SerializeField] TMP_Text estimateCost;
    [Space()]
    [SerializeField] Sprite unknownSprite;

    Dictionary<int, int> items;
    int activeItemID = -1;
    int offset = 0;

    public int GetActiveItemID() => activeItemID;
    public void SetActiveItemID(int activeItemID) => this.activeItemID = activeItemID;

    public void UpdateActiveItem()
    {
        foreach (InventoryItem item in itemElements)
        {
            if (item.IsPointerOnItem())
            {
                activeItemID = item.GetID();
                break;
            }
        }
        // Debug.Log($"inventory layout element: active item id = {activeItemID}");
    }

    public void UpdateLayout(Dictionary<int, int> items)
    {
        this.items = items
        .OrderBy(x => lootCategoryManager.lootCategories.FirstOrDefault(lc => lc.id == x.Key)?.sortGroup ?? int.MaxValue)
        .ThenBy(x => x.Key)
        .ToDictionary(x => x.Key, x => x.Value);

        print(this.items);
        while (items.Count - offset + offsetStep <= itemElements.Count && offset > 0)
        {
            ScrollUp(update: false);
        }
        int itemIndex = -offset;
        int totalCost = 0;
        foreach (int i in this.items.Keys)
        {
            LootCategory lc = lootCategoryManager.lootCategories.FirstOrDefault((x) => x.id == i);

            if (itemIndex >= 0)
            {
                itemElements[itemIndex].SetActive(true);

                Sprite itemSprite = lc.sprite;
                if (itemSprite == null) itemSprite = unknownSprite;

                itemElements[itemIndex].Initialize(i, itemSprite, items[i], lc);
                //itemElements[itemIndex].UpdateTooltip();
            }

            itemIndex++;

            if (itemIndex == itemElements.Count) break;
        }

        for (; itemIndex < itemElements.Count; itemIndex++)
        {
            itemElements[itemIndex].SetActive(false);
        }

        foreach (int i in this.items.Keys)
        {
            LootCategory lc = lootCategoryManager.lootCategories.FirstOrDefault((x) => x.id == i);
            totalCost += lc.cost * items[i];
        }

        UpdateActiveItem();

        if (hasVolume) // inventory
        {
            float currentVolume = InventorySystem.Instance.GetOccupiedVolume();
            float maxVolume = InventorySystem.Instance.GetMaxVolume();

            float ratio = currentVolume / maxVolume * 100;
            string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

            totalVolume.text = $"{NumberFormatter.FormatNumber(currentVolume * 1000)} / {NumberFormatter.FormatNumber(maxVolume * 1000)} л ({ratio.ToString(format)}%)";
            volumePB.SetMaxValue(maxVolume);
            volumePB.SetProgress(currentVolume);
        }

        if (hasCount) // chest
        {
            float currentCount = Chest.Instance.GetTotalItemsAmount();
            float maxCount = Chest.Instance.GetMaxItemsAmount();

            float ratio = currentCount / maxCount * 100;
            string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

            totalCount.text = $"{NumberFormatter.FormatNumber(currentCount)} / {NumberFormatter.FormatNumber(maxCount)} ({ratio.ToString(format)}%)";
            countPB.SetMaxValue(maxCount);
            countPB.SetProgress(currentCount);
        }

        if (hasEstimateCost)
        {
            if (totalCost < 2_000_000_000)
                estimateCost.text = $"Оценочная стоимость вещей: {NumberFormatter.FormatNumberWithGrouping(totalCost)} UMU";
            else
                estimateCost.text = $"Оценочная стоимость вещей: более 2 млрд UMU";
        }
    }

    void ModifyOffset(int diff, bool update = true)
    {
        offset += diff;
        if (offset < 0) offset = 0;
        if (update)
            UpdateLayout(items);
    }

    public void ScrollDown(bool update = true)
    {
        if (items.Count - offset > itemElements.Count)
        {
            ModifyOffset(offsetStep, update);
        }
    }

    public void ScrollUp(bool update = true)
    {
        if (offset > 0)
            ModifyOffset(-offsetStep, update);
    }

    public void ClearOffset()
    {
        offset = 0;
    }
}
