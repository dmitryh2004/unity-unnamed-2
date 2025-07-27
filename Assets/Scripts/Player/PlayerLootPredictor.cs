using UnityEngine;

public class PlayerLootPredictor : UpgradableItem
{
    public static PlayerLootPredictor Instance = null;
    public int GetPrecision()
    {
        return (int) GetUpgradableValue1();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
