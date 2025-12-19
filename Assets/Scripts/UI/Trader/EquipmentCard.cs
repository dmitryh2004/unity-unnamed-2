using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum Equipments
{
    Inventory = 0,
    Virus = 1,
    Scanner = 2,
    Flashlight = 3
}
public class EquipmentCard : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] TMP_Text equipmentName;
    [SerializeField] TMP_Text equipmentLevel;
    [SerializeField] TMP_Text equipmentUpgradeCost;
    [Space]
    [SerializeField] Equipments equipment;
    [Space]
    [SerializeField] Image background;
    [Header("Panel colors")]
    [SerializeField] Color cantUpgradeColor = new Color(.5f, .5f, .5f, .2f);
    [SerializeField] Color canUpgradeColor = new Color(.0f, 1f, .0f, .2f);
    [SerializeField] Color maxUpgradedColor = new Color(1f, 1f, .0f, .2f);
    [SerializeField] Color selectedColor = new Color(.0f, 1f, .0f, .5f);

    bool selected = false;

    public bool IsSelected() => selected;
    public void SetSelected(bool selected) => this.selected = selected;

    public UpgradableItem GetEquipment()
    {
        switch (equipment)
        {
            case Equipments.Inventory:
                return InventorySystem.Instance;
            case Equipments.Virus:
                return VirusController.Instance;
            case Equipments.Scanner:
                return PlayerScanner.Instance;
            case Equipments.Flashlight:
                return PlayerFlashlight.Instance;
            default:
                return null;
        }
    }

    public void UpdateCard()
    {
        UpgradableItem equipmentObject = GetEquipment();

        if (equipmentObject == null) return;

        equipmentName.text = equipmentObject.GetName();

        int level = equipmentObject.GetLevel();
        int maxLevel = equipmentObject.GetMaxLevel();

        bool maxUpgraded = level == maxLevel;
        bool canUpgrade = false;

        if (!maxUpgraded)
        {
            equipmentLevel.text = $"Уровень: {level} / {maxLevel}";

            int upgradeCost = equipmentObject.GetUpgradeCost(level + 1);

            canUpgrade = PlayerWallet.Instance.CanAfford(upgradeCost);
            equipmentUpgradeCost.text = $"Цена улучшения: {NumberFormatter.FormatNumberWithGrouping(upgradeCost)} UMU";
        }
        else
        {
            equipmentLevel.text = "Максимально улучшено";
            equipmentUpgradeCost.text = "";
        }

        UpdateColor(maxUpgraded, canUpgrade);
    }

    void UpdateColor(bool maxUpgraded, bool canUpgrade)
    {
        if (selected)
        {
            background.color = selectedColor;
        }
        else
        {
            if (maxUpgraded)
            {
                background.color = maxUpgradedColor;
            }
            else
            {
                if (canUpgrade)
                {
                    background.color = canUpgradeColor;
                }
                else
                {
                    background.color = cantUpgradeColor;
                }
            }
        }
    }
}
