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

    public float GetUpgradableValue1()
    {
        return upgradableItemData.upgradableValue1List.FirstOrDefault((x) => x.level == level).value;
    }

    public float GetUpgradableValue2()
    {
        return upgradableItemData.upgradableValue2List.FirstOrDefault((x) => x.level == level).value;
    }

    public float GetUpgradableValue3()
    {
        return upgradableItemData.upgradableValue3List.FirstOrDefault((x) => x.level == level).value;
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
    public string GetUV1Name() => upgradableItemData.uv1Name;
    public string GetUV2Name() => upgradableItemData.uv2Name;
    public string GetUV3Name() => upgradableItemData.uv3Name;
}
