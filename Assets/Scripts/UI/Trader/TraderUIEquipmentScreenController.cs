using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderUIEquipmentScreenController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    [Space]
    [SerializeField] List<EquipmentCard> equipmentCards;
    [Space]
    [SerializeField] TMP_Text yourBalance;
    [Header("Equipment Info")]
    [SerializeField] GameObject chosenEquipmentInfo;
    [SerializeField] GameObject noChosenEquipment;
    [Space]
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text characteristicsTitle;
    [SerializeField] TMP_Text characteristics;
    [SerializeField] TMP_Text upgradeEffectTitle;
    [SerializeField] TMP_Text upgradeEffect;
    [SerializeField] TMP_Text additionalInfo;
    [Space]
    [SerializeField] Button upgradeButton;
    [SerializeField] TMP_Text upgradeButtonText;

    int selectedItem = -1;
    ScrollRect chosenEquipmentInfoScrollRect;

    private void Awake()
    {
        chosenEquipmentInfoScrollRect = chosenEquipmentInfo.GetComponent<ScrollRect>();
    }

    public override void OnShow()
    {
        base.OnShow();
        selectedItem = -1;
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

        yourBalance.text = $"Ваш баланс: {NumberFormatter.FormatNumberWithGrouping(PlayerWallet.Instance.GetMoney())} UMU";

        UpdateSelectedItemInfo();
    }

    public void SetSelectedItem(int item)
    {
        if (item < 0 || item >= equipmentCards.Count) return;
        selectedItem = item;

        UpdateWindow();
    }

    void UpdateSelectedItemInfo()
    {
        chosenEquipmentInfo.SetActive(selectedItem != -1);
        upgradeButton.gameObject.SetActive(selectedItem != -1);
        noChosenEquipment.SetActive(selectedItem == -1);

        if (selectedItem == -1) return;

        UpgradableItem equipment = equipmentCards[selectedItem].GetEquipment();
        title.text = equipment.GetName();
        desc.text = equipment.GetDesc();
        characteristicsTitle.text = $"Характеристики (уровень {equipment.GetLevel()})";

        string characteristics = "";
        List<string> chars = new();

        for (int i = 0; i < equipment.GetUpgradableValuesCount(); i++)
        {
            float? uvValue = equipment.GetUpgradableValue(i);
            
            if (uvValue != null)
                chars.Add($"{equipment.GetUpgradableValueName(i)}: {NumberFormatter.FormatNumberWithGrouping((float)uvValue * (float)equipment.GetUpgradableValueShowMultiplier(i))} {equipment.GetUpgradableValueSuffix(i)}");
        }

        characteristics = (chars.Count > 0) ? string.Join("\n", chars) : "Данный предмет еще не куплен";
        this.characteristics.text = characteristics;

        bool maxUpgraded = equipment.GetLevel() == equipment.GetMaxLevel();

        upgradeEffectTitle.gameObject.SetActive(!maxUpgraded);
        upgradeEffect.gameObject.SetActive(!maxUpgraded);
        upgradeButton.gameObject.SetActive(selectedItem != -1 && !maxUpgraded && PlayerWallet.Instance.CanAfford(equipment.GetUpgradeCost(equipment.GetLevel() + 1)));

        if (!maxUpgraded)
        {
            upgradeEffectTitle.text = $"Эффекты от улучшения (уровень {equipment.GetLevel() + 1})";
            

            List<string> ue = new ();
            for (int i = 0; i < equipment.GetUpgradableValuesCount(); i++)
            {
                AddUpgradeEffectString(equipment, ue, i);
            }

            string upgradeEffects = string.Join("\n", ue);
            upgradeEffect.text = upgradeEffects;

            upgradeButtonText.text = $"Улучшить ({NumberFormatter.FormatNumberWithGrouping(equipment.GetUpgradeCost(equipment.GetLevel() + 1))} UMU)";
        }

        additionalInfo.text = equipment.GetAdditionalInfo();

        chosenEquipmentInfoScrollRect.verticalNormalizedPosition = 1f;
    }

    private void AddUpgradeEffectString(UpgradableItem equipment, List<string> ue, int uvNumber)
    {
        float? curUV = equipment.GetUpgradableValue(uvNumber), nextUV = equipment.GetUpgradableValue(uvNumber, equipment.GetLevel() + 1);
        float? showMultiplier = equipment.GetUpgradableValueShowMultiplier(uvNumber);
        string UVName = equipment.GetUpgradableValueName(uvNumber), UVSuffix = equipment.GetUpgradableValueSuffix(uvNumber);
        bool? increaseIsPositive = equipment.UpgradableValueIncreaseIsPositive(uvNumber);

        if (curUV == null)
        {
            if (nextUV != null)
            {
                ue.Add($"<color=#ffff00>(Новое свойство)</color> {UVName}: {NumberFormatter.FormatNumberWithGrouping((float)nextUV * (float)showMultiplier)} {UVSuffix}");
            }
            return;
        }
        float diff = (float)nextUV - (float)curUV;

        if (diff != 0)
        {
            bool positive = ((bool)increaseIsPositive) ? diff > 0 : diff < 0;
            string sign = (diff > 0) ? "+" : "";
            string colorTag = (positive) ? "#00FF00" : "#FF0000";
            ue.Add($"{UVName}: <color={colorTag}>{sign}{NumberFormatter.FormatNumberWithGrouping(diff * (float)showMultiplier)} {UVSuffix}</color>");
        }
    }

    public void UpgradeEquipment()
    {
        if (selectedItem < 0 || selectedItem > 3) return;
        UpgradableItem equipment = equipmentCards[selectedItem].GetEquipment();
        int upgradeCost = equipment.GetUpgradeCost(equipment.GetLevel() + 1);
        if (PlayerWallet.Instance.CanAfford(upgradeCost))
        {
            equipment.SetLevel(equipment.GetLevel() + 1);
            PlayerWallet.Instance.SubtractMoney(upgradeCost);
        }

        UpdateWindow();
    }
}
