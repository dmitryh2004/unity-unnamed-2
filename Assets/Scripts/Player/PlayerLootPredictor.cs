using UnityEngine;

public class PlayerLootPredictor : UpgradableItem
{
    public static PlayerLootPredictor Instance = null;
    public int GetLootPrecision()
    {
        float? uv = GetUpgradableValue1();
        if (uv == null) return -1;
        return (int)uv;
    }

    public int GetProtectedRoomPrecision()
    {
        float? uv = GetUpgradableValue2();
        if (uv == null) return -1;
        return (int)uv;
    }

    public int GetSecuredRoomPrecision()
    {
        float? uv = GetUpgradableValue3();
        if (uv == null) return -1;
        return (int)uv;
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
