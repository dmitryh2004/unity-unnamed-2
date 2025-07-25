using System;
using UnityEngine;

public class TraderUIQuotaScreenController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    [Space(10)]
    [SerializeField] TraderObject trader;
    [Header("Selected task")]
    [SerializeField] TaskInfoElement taskInfoElement;

    public override void OnShow()
    {
        base.OnShow();
        UpdateTaskInfo();
        UpdateInventoryItems();
    }

    private void UpdateTaskInfo()
    {
        Order currentOrder = QuotaSystem.Instance.GetOrder();
        taskInfoElement.UpdateTaskInfo(currentOrder);
    }

    private void UpdateInventoryItems()
    {
        throw new NotImplementedException();
    }
}
