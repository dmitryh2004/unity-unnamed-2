using System;
using System.Collections.Generic;
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

    public override void OnShow()
    {
        base.OnShow();
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
