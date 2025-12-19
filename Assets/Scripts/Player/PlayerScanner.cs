using UnityEngine;

public class PlayerScanner : UpgradableItem
{
    public static PlayerScanner Instance = null;
    public int GetLootPrecision()
    {
        float? uv = GetUpgradableValue(0);
        if (uv == null) return -1;
        return (int)uv;
    }

    public int GetProtectedRoomPrecision()
    {
        float? uv = GetUpgradableValue(1);
        if (uv == null) return -1;
        return (int)uv;
    }

    public int GetSecuredRoomPrecision()
    {
        float? uv = GetUpgradableValue(2);
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
