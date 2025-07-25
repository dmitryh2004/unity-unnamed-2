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
    [SerializeField] bool hasEstimateCost;
    [SerializeField] TMP_Text estimateCost;
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
        Debug.Log($"inventory layout element: active item id = {activeItemID}");
    }

    public void UpdateLayout(Dictionary<int, int> items)
    {
        this.items = items;
        int itemIndex = itemElements.Count - 1 + offset;
        int totalCost = 0;
        foreach (int i in items.Keys)
        {
            LootCategory lc = lootCategoryManager.lootCategories.FirstOrDefault((x) => x.id == i);

            if (itemIndex < itemElements.Count)
            {
                itemElements[itemIndex].SetActive(true);

                Sprite itemSprite = lc.sprite;
                if (itemSprite == null) itemSprite = unknownSprite;

                itemElements[itemIndex].Initialize(i, itemSprite, items[i]);
                itemElements[itemIndex].UpdateTooltip(lc);
            }

            itemIndex--;

            if (itemIndex < 0) break;
        }

        for (; itemIndex >= 0; itemIndex--)
        {
            itemElements[itemIndex].SetActive(false);
        }

        foreach (int i in items.Keys)
        {
            LootCategory lc = lootCategoryManager.lootCategories.FirstOrDefault((x) => x.id == i);
            totalCost += lc.cost * items[i];
        }

        UpdateActiveItem();

        if (hasVolume)
        {
            float currentVolume = InventorySystem.Instance.GetOccupiedVolume();
            float maxVolume = InventorySystem.Instance.GetMaxVolume();

            float ratio = currentVolume / maxVolume * 100;
            string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

            totalVolume.text = $"{NumberFormatter.FormatNumber(currentVolume * 1000)} / {NumberFormatter.FormatNumber(maxVolume * 1000)} л ({ratio.ToString(format)}%)";
            volumePB.SetMaxValue(maxVolume);
            volumePB.SetProgress(currentVolume);
        }
        
        if (hasEstimateCost)
        {
            if (totalCost < 2_000_000_000)
                estimateCost.text = $"ќценочна€ стоимость вещей: {NumberFormatter.FormatNumberWithGrouping(totalCost)} руб.";
            else
                estimateCost.text = $"ќценочна€ стоимость вещей: более 2 млрд руб.";
        }
    }

    void ModifyOffset(int diff)
    {
        offset += diff;
        if (offset < 0) offset = 0;
        UpdateLayout(items);
    }

    public void ScrollDown()
    {
        if (items.Count - offset > itemElements.Count)
        {
            ModifyOffset(offsetStep);
        }
    }

    public void ScrollUp()
    {
        if (offset > 0)
            ModifyOffset(-offsetStep);
    }

    public void ClearOffset()
    {
        offset = 0;
    }
}
