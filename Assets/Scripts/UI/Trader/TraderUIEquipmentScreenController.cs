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
        if (equipment.HasUV1() && equipment.GetUpgradableValue1() != null)
        {
            chars.Add($"{equipment.GetUV1Name()}: {NumberFormatter.FormatNumberWithGrouping((float)equipment.GetUpgradableValue1() * equipment.GetUV1ShowMultiplier())} {equipment.GetUV1Suffix()}");
        }
        if (equipment.HasUV2() && equipment.GetUpgradableValue2() != null)
        {
            chars.Add($"{equipment.GetUV2Name()}: {NumberFormatter.FormatNumberWithGrouping((float)equipment.GetUpgradableValue2() * equipment.GetUV2ShowMultiplier())} {equipment.GetUV2Suffix()}");
        }
        if (equipment.HasUV3() && equipment.GetUpgradableValue3() != null)
        {
            chars.Add($"{equipment.GetUV3Name()}: {NumberFormatter.FormatNumberWithGrouping((float)equipment.GetUpgradableValue3() * equipment.GetUV3ShowMultiplier())} {equipment.GetUV3Suffix()}");
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
            if (equipment.HasUV1())
            {
                AddUpgradeEffectString(equipment, ue, 1);
            }
            if (equipment.HasUV2())
            {
                AddUpgradeEffectString(equipment, ue, 2);
            }
            if (equipment.HasUV3())
            {
                AddUpgradeEffectString(equipment, ue, 3);
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
        float? curUV, nextUV;
        float showMultiplier;
        string UVName, UVSuffix;
        bool increaseIsPositive;

        switch (uvNumber)
        {
            case 1:
                curUV = equipment.GetUpgradableValue1();
                nextUV = equipment.GetUpgradableValue1(equipment.GetLevel() + 1);
                showMultiplier = equipment.GetUV1ShowMultiplier();
                UVName = equipment.GetUV1Name();
                UVSuffix = equipment.GetUV1Suffix();
                increaseIsPositive = equipment.UV1IncreaseIsPositive();
                break;
            case 2:
                curUV = equipment.GetUpgradableValue2();
                nextUV = equipment.GetUpgradableValue2(equipment.GetLevel() + 1);
                showMultiplier = equipment.GetUV2ShowMultiplier();
                UVName = equipment.GetUV2Name();
                UVSuffix = equipment.GetUV2Suffix();
                increaseIsPositive = equipment.UV2IncreaseIsPositive();
                break;
            case 3:
                curUV = equipment.GetUpgradableValue3();
                nextUV = equipment.GetUpgradableValue3(equipment.GetLevel() + 1);
                showMultiplier = equipment.GetUV3ShowMultiplier();
                UVName = equipment.GetUV3Name();
                UVSuffix = equipment.GetUV3Suffix();
                increaseIsPositive = equipment.UV3IncreaseIsPositive();
                break;
            default:
                return;
        }

        if (curUV == null)
        {
            if (nextUV != null)
            {
                ue.Add($"<color=#ffff00>(Новое свойство)</color> {UVName}: {NumberFormatter.FormatNumberWithGrouping((float)nextUV * showMultiplier)} {UVSuffix}");
            }
            return;
        }
        float diff = (float)nextUV - (float)curUV;

        if (diff != 0)
        {
            bool positive = (increaseIsPositive) ? diff > 0 : diff < 0;
            string sign = (diff > 0) ? "+" : "";
            string colorTag = (positive) ? "#00FF00" : "#FF0000";
            ue.Add($"{UVName}: <color={colorTag}>{sign}{NumberFormatter.FormatNumberWithGrouping(diff * showMultiplier)} {UVSuffix}</color>");
        }
    }

    public void UpgradeEquipment()
    {
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
