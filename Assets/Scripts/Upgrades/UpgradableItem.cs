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

    public int GetUpgradableValuesCount() => upgradableItemData.upgradableValues.Count;

    public float? GetUpgradableValue(int index)
    {
        if (index < 0 || index > GetUpgradableValuesCount()) return null;
        UpgradableValue uv = upgradableItemData.upgradableValues[index].upgradableValueList.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public float? GetUpgradableValue(int index, int level)
    {
        if (index < 0 || index > upgradableItemData.upgradableValues.Count) return null;
        UpgradableValue uv = upgradableItemData.upgradableValues[index].upgradableValueList.Find((x) => x.level == level);
        if (uv == null) return null;
        return uv.value;
    }

    public bool? UpgradableValueIncreaseIsPositive(int index)
    {
        if (index < 0 || index > upgradableItemData.upgradableValues.Count) return null;
        return upgradableItemData.upgradableValues[index].increaseIsPositive;
    }

    public string? GetUpgradableValueName(int index)
    {
        if (index < 0 || index > upgradableItemData.upgradableValues.Count) return null;
        return upgradableItemData.upgradableValues[index].name;
    }

    public string? GetUpgradableValueSuffix(int index)
    {
        if (index < 0 || index > upgradableItemData.upgradableValues.Count) return null;
        return upgradableItemData.upgradableValues[index].uvSuffix;
    }

    public float? GetUpgradableValueShowMultiplier(int index)
    {
        if (index < 0 || index > upgradableItemData.upgradableValues.Count) return null;
        return upgradableItemData.upgradableValues[index].uvShowMultiplier;
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
    public string GetAdditionalInfo() => upgradableItemData.additionalInfo;
}
