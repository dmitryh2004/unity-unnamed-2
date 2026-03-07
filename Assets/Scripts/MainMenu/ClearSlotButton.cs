using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearSlotButton : HoldButtonController
{
    [SerializeField] int slotNumber;
    [SerializeField] SaveManager saveManager;
    [SerializeField] MainMenuSlotUIController slotUIController;

    protected override void OnHoldComplete()
    {
        
        saveManager.ClearSave(slotNumber);
        slotUIController.RefreshData();
    }
}
