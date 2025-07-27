using System.Collections.Generic;
using UnityEngine;

public class TraderUIEquipmentScreenController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    [Space]
    [SerializeField] List<EquipmentCard> equipmentCards;

    int selectedItem = -1;

    public override void OnShow()
    {
        base.OnShow();
        UpdateWindow();
    }

    void UpdateWindow()
    {
        for (int i = 0; i < equipmentCards.Count; i++)
        {
            var v = equipmentCards[i];
            v.SetSelected(i == selectedItem);
            v.UpdateCard();
        }
    }

    public void SetSelectedItem(int item)
    {
        if (item < 0 || item >= equipmentCards.Count) return;
        selectedItem = item;

        UpdateWindow();
    }
}
