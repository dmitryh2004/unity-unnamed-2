using System.Linq;
using UnityEngine;

public class UpgradableItem : MonoBehaviour
{
    protected int level;
    [SerializeField] int defaultLevel = 1;
    [SerializeField] UpgradeCostManager upgradeCostManager;

    public int GetLevel() => level;
    public void SetLevel(int level)
    {
        if (upgradeCostManager.minLevel <= level && level <= upgradeCostManager.maxLevel)
        {
            this.level = level;
        }
        else if (level < upgradeCostManager.minLevel)
        {
            this.level = upgradeCostManager.minLevel;
        }
        else
        {
            this.level = upgradeCostManager.maxLevel;
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
        return upgradeCostManager.upgradableValue1List.FirstOrDefault((x) => x.level == level).value;
    }

    public float GetUpgradableValue2()
    {
        return upgradeCostManager.upgradableValue2List.FirstOrDefault((x) => x.level == level).value;
    }

    public float GetUpgradableValue3()
    {
        return upgradeCostManager.upgradableValue3List.FirstOrDefault((x) => x.level == level).value;
    }
}
