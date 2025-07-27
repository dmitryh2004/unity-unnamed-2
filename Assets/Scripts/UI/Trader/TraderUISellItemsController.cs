using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TraderUISellItemsController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    [Space()]
    [SerializeField] InventoryLayoutElement inventoryLayoutElement;
    [Space()]
    [SerializeField] TMP_Text currentBalance;
    Dictionary<int, int> items;
    int activeItemID = -1;

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
    public override void OnShow()
    {
        base.OnShow();
        UpdateWindow();
    }

    public void SetActiveItem(int id)
    {
        activeItemID = id;
        inventoryLayoutElement.SetActiveItemID(id);
    }

    public void UpdateActiveItem()
    {
        inventoryLayoutElement.UpdateActiveItem();
        activeItemID = inventoryLayoutElement.GetActiveItemID();
    }

    void UpdateWindow()
    {
        items = InventorySystem.Instance.GetItems();

        int balance = PlayerWallet.Instance.GetMoney();
        currentBalance.text = $"Текущий баланс: {NumberFormatter.FormatNumberWithGrouping(balance)} UMU";

        inventoryLayoutElement.UpdateLayout(items);
    }

    public void SellActiveItem(bool all = false)
    {
        if (activeItemID == -1)
        {
            Debug.Log("Sell item: no active item");
            return;
        }

        if (!items.ContainsKey(activeItemID))
        {
            Debug.LogWarning($"Found an error while selling item: active item not present in dictionary ({GetItemsString()})");
            return;
        }

        if (items[activeItemID] <= 0)
        {
            Debug.LogWarning($"Found an error while selling item: active item count ({items[activeItemID]}) <= 0");
            return;
        }

        LootCategory lc = InventorySystem.Instance.GetLootCategoryById(activeItemID);
        int cost = lc.cost;
        int count = all ? items[activeItemID] : 1;
        InventorySystem.Instance.RemoveItem(lc, all);
        PlayerWallet.Instance.AddMoney(cost * count);

        UpdateWindow();
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
