using UnityEngine;

public class PlayerLootPredictor : MonoBehaviour
{
    public static PlayerLootPredictor Instance = null;
    [SerializeField][Range(0, 5)] int level;
    int minLevel = 0, maxLevel = 5;

    public int GetLevel() => level;
    public int GetPrecision()
    {
        switch (level)
        {
            case 1:
                return 1000000;
            case 2:
                return 500000;
            case 3:
                return 250000;
            case 4:
                return 100000;
            case 5:
                return 0;
            default:
                return -1;
        }
    }
    public void SetLevel(int level) 
    {
        if (level < minLevel)
        {
            this.level = minLevel;
            Debug.LogWarning($"Loot predictor: level clamped to {minLevel} ({level} < {minLevel})");
        }
        else if (level > maxLevel)
        {
            this.level = maxLevel;
            Debug.LogWarning($"Loot predictor: level clamped to {maxLevel} ({level} > {maxLevel})");
        }
        else
            this.level = level;
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
