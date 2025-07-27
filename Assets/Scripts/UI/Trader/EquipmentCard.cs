using System;
using TMPro;
using UnityEngine;

[Serializable]
public enum Equipments
{
    Inventory = 0,
    Virus = 1,
    LootPredictor = 2
}
public class EquipmentCard : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] TMP_Text equipmentName;
    [SerializeField] TMP_Text equipmentLevel;
    [SerializeField] TMP_Text equipmentUpgradeCost;
    [Space]
    [SerializeField] Equipments equipment;

    public void UpdateCard()
    {
        UpgradableItem equipmentObject = null;
        switch (equipment)
        {
            case Equipments.Inventory:
                equipmentObject = InventorySystem.Instance;
                break;
            case Equipments.Virus:
                equipmentObject = VirusController.Instance;
                break;
            case Equipments.LootPredictor:
                equipmentObject = PlayerLootPredictor.Instance;
                break;
        }

        equipmentName.text = equipmentObject.GetName();

        int level = equipmentObject.GetLevel();
        int maxLevel = equipmentObject.GetMaxLevel();
        if (level < maxLevel)
        {
            equipmentLevel.text = $"Уровень: {level} / {maxLevel}";

            int upgradeCost = equipmentObject.GetCurrentUpgradeCost();
            equipmentUpgradeCost.text = $"Цена улучшения: {NumberFormatter.FormatNumberWithGrouping(upgradeCost)} UMU";
        }
        else
            equipmentLevel.text = "Максимально улучшено";
    }
}
