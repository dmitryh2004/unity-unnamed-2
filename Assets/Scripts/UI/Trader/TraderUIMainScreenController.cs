using UnityEngine;

public class TraderUIMainScreenController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    
    public void ChangeToQuotaScreen()
    {
        if (QuotaSystem.Instance.HasUncompletedOrder())
        {
            windowController.SetScreen(1);
        }
        else
        {
            windowController.SetScreen(2);
        }
    }

    public void ChangeToSellScreen()
    {
        windowController.SetScreen(3);
    }

    public void ChangeToEquipmentScreen()
    {
        windowController.SetScreen(4);
    }
}
