using System.Linq;
using UnityEngine;

public class UpgradableItem : MonoBehaviour
{
    protected int level;
    [SerializeField] int defaultLevel = 1;
    [SerializeField] UpgradableItemData upgradableItemData;

    public int GetLevel() => level;
    public int GetMaxLevel() => upgradableItemData.maxLevel;
    public void SetLevel(int level)
    {
        if (upgradableItemData.minLevel <= level && level <= upgradableItemData.maxLevel)
        {
            this.level = level;
        }
        else if (level < upgradableItemData.minLevel)
        {
            this.level = upgradableItemData.minLevel;
        }
        else
        {
            this.level = upgradableItemData.maxLevel;
        }
        OnSetLevel();
    }

    protected virtual void OnSetLevel()
    {

    }

    protected void InitLevel()
    {
        level = defaultLevel;
    }

    public float? GetUpgradableValue1()
    {
        UpgradableValue uv = upgradableItemData.upgradableValue1List.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public float? GetUpgradableValue2()
    {
        UpgradableValue uv = upgradableItemData.upgradableValue2List.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public float? GetUpgradableValue3()
    {
        UpgradableValue uv = upgradableItemData.upgradableValue3List.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public float? GetUpgradableValue1(int level)
    {
        UpgradableValue uv = upgradableItemData.upgradableValue1List.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public float? GetUpgradableValue2(int level)
    {
        UpgradableValue uv = upgradableItemData.upgradableValue2List.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public float? GetUpgradableValue3(int level)
    {
        UpgradableValue uv = upgradableItemData.upgradableValue3List.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public int GetUpgradeCost(int level)
    {
        UpgradeCost upgradeCost = upgradableItemData.upgradeCosts.FirstOrDefault((x) => x.level == level);
        if (upgradeCost != null)
        {
            return upgradeCost.cost;
        }
        else return -1;
    }
    public string GetName() => upgradableItemData.itemName;
    public string GetDesc() => upgradableItemData.description;
    public bool HasUV1() => upgradableItemData.hasUpgradableValue1;
    public bool HasUV2() => upgradableItemData.hasUpgradableValue2;
    public bool HasUV3() => upgradableItemData.hasUpgradableValue3;
    public bool UV1IncreaseIsPositive() => upgradableItemData.increaseIsPositive1;
    public bool UV2IncreaseIsPositive() => upgradableItemData.increaseIsPositive2;
    public bool UV3IncreaseIsPositive() => upgradableItemData.increaseIsPositive3;
    public string GetUV1Name() => upgradableItemData.uv1Name;
    public string GetUV2Name() => upgradableItemData.uv2Name;
    public string GetUV3Name() => upgradableItemData.uv3Name;

    public float GetUV1ShowMultiplier() => upgradableItemData.uv1ShowMultiplier;
    public float GetUV2ShowMultiplier() => upgradableItemData.uv2ShowMultiplier;
    public float GetUV3ShowMultiplier() => upgradableItemData.uv3ShowMultiplier;
    public string GetUV1Suffix() => upgradableItemData.uv1Suffix;
    public string GetUV2Suffix() => upgradableItemData.uv2Suffix;
    public string GetUV3Suffix() => upgradableItemData.uv3Suffix;

    public string GetAdditionalInfo() => upgradableItemData.additionalInfo;
}
