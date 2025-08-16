using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraderUIQuotaScreenController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    [Space(10)]
    [SerializeField] TraderObject trader;
    [Header("Selected task")]
    [SerializeField] TaskInfoElement taskInfoElement;
    [SerializeField] InventoryLayoutElement inventoryLayoutElement;
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

    void UpdateWindow()
    {
        UpdateTaskInfo();
        UpdateInventoryItems();
    }

    public void UpdateActiveItem()
    {
        inventoryLayoutElement.UpdateActiveItem();
        activeItemID = inventoryLayoutElement.GetActiveItemID();
    }

    public void SetActiveItem(int id)
    {
        activeItemID = id;
        inventoryLayoutElement.SetActiveItemID(id);
    }

    private void UpdateTaskInfo()
    {
        Order currentOrder = QuotaSystem.Instance.GetOrder();
        taskInfoElement.UpdateTaskInfo(currentOrder);
    }

    private void UpdateInventoryItems()
    {
        items = InventorySystem.Instance.GetItems();
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
        ClientType ct = QuotaSystem.Instance.GetOrder().GetClientType();
        LootCostModifier lcm = ct.lootCostModifiers.FirstOrDefault((x) => x.itemID == activeItemID);
        
        int costWithModifiers = lc.cost;
        if (lcm != null) costWithModifiers = (int)(costWithModifiers * lcm.modifier);

        bool quotaCompleted = false;

        if (all)
        {
            while (items.ContainsKey(activeItemID))
            {
                InventorySystem.Instance.RemoveItem(lc);
                QuotaSystem.Instance.SetCollected(QuotaSystem.Instance.GetCollected() + costWithModifiers);
                if (QuotaSystem.Instance.HasCompletedOrder())
                {
                    quotaCompleted = true;
                    break;
                }
            }
        }
        else
        {
            InventorySystem.Instance.RemoveItem(lc);
            QuotaSystem.Instance.SetCollected(QuotaSystem.Instance.GetCollected() + costWithModifiers);
            if (QuotaSystem.Instance.HasCompletedOrder())
            {
                quotaCompleted = true;
            }
           
        }

        QuotaSystem.Instance.UpdateUI();

        if (quotaCompleted)
        {
            if (ct.hasEarlyCompletionBonus)
            {
                int daysLeft = QuotaSystem.Instance.GetDaysLeft();
                if (daysLeft >= 2)
                {
                    PlayerWallet.Instance.AddMoney((int)(QuotaSystem.Instance.GetRequired() * ct.earlyCompletionBonusModifier2));
                }
                else if (daysLeft >= 1)
                {
                    PlayerWallet.Instance.AddMoney((int)(QuotaSystem.Instance.GetRequired() * ct.earlyCompletionBonusModifier1));
                }
            }

            trader.IncreaseMultiplier();
            trader.GenerateOrders();
            windowController.SetScreen(2);
            LevelManager.Instance.SaveGame(true);
        }
        else 
        {
            UpdateWindow();
            UpdateActiveItem();
        }
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
