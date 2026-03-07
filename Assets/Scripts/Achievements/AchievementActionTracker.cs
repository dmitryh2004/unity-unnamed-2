using System.Collections.Generic;
using UnityEngine;

public class AchievementActionTracker : MonoBehaviour
{
    public static AchievementActionTracker Instance = null;
    [SerializeField] LootCategoryManager lootCategoryManager;
    [Header("Location achievements")]
    [SerializeField] List<string> locationNames = new ();
    [SerializeField] List<int> requiredLootSumCollected = new ();

    [Header("Collectonaire achievements")]
    [SerializeField] List<int> achievementDataCollectionaireBooksRare;
    [SerializeField] List<int> achievementDataCollectionaireBooksCollectional;
    [SerializeField] List<int> achievementDataCollectionaireStones;
    [SerializeField] List<int> achievementDataCollectionaireMoney;
    Dictionary<int, bool> foundItems = new();

    [Header("Guardian sound achievement")]
    [SerializeField] int guardianSoundsCount = 10;
    Dictionary<int, bool> foundGuardianSounds = new ();

    [Header("Chest achievements")]
    [SerializeField] int maxChestItemsCount = 1000;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        // found items
        ReadFoundItemsData();

        // found guardian sounds
        ReadFoundGuardianSoundsData();
    }

    private void ReadFoundGuardianSoundsData()
    {
        string foundSounds = PlayerPrefs.GetString("Achievement_FoundGuardianSounds", "");
        if (foundSounds == "")
        {
            for (int i = 0; i < guardianSoundsCount; i++)
            {
                this.foundGuardianSounds[i] = false;
            }
        }
        else
        {
            char[] letters = foundSounds.ToCharArray();
            for (int i = 0; i < guardianSoundsCount; i++)
            {
                this.foundGuardianSounds[i] = (letters[i] == '1');
            }
        }
    }

    void ReadFoundItemsData()
    {
        string foundItems = PlayerPrefs.GetString("Achievement_FoundItems", "");
        if (foundItems == "")
        {
            for (int i = 1; i <= lootCategoryManager.lootCategories.Count; i++)
            {
                this.foundItems[i] = false;
            }
        }
        else
        {
            char[] letters = foundItems.ToCharArray();
            for (int i = 0; i < lootCategoryManager.lootCategories.Count; i++)
            {
                this.foundItems[i + 1] = (letters[i] == '1');
            }
        }
    }

    public void OnItemPickedUp(LootCategory lootCategory)
    {
        bool isPickedUpEarlier = foundItems.ContainsKey(lootCategory.id) ? foundItems[lootCategory.id] : false;
        if (!isPickedUpEarlier)
        {
            foundItems[lootCategory.id] = true;
            PlayerPrefs.SetString("Achievement_FoundItems", GetFoundItemsData());
            PlayerPrefs.Save();

            AchievementSystem.Instance.SetAchievementProgress("collectionaire_books_rare", GetLootAchievementProgress("collectionaire_books_rare"));
            AchievementSystem.Instance.SetAchievementProgress("collectionaire_books_collectional", GetLootAchievementProgress("collectionaire_books_collectional"));
            AchievementSystem.Instance.SetAchievementProgress("collectionaire_stones", GetLootAchievementProgress("collectionaire_stones"));
            AchievementSystem.Instance.SetAchievementProgress("collectionaire_money", GetLootAchievementProgress("collectionaire_money"));
            AchievementSystem.Instance.SetAchievementProgress("collectionaire_all", GetLootAchievementProgress("collectionaire_all"));
        }
    }

    public void OnChestContentChanged(int itemCount, int chestLootCost)
    {
        if (itemCount == maxChestItemsCount)
        {
            AchievementSystem.Instance.SetAchievementProgress("full_chest", 1);
        }
        AchievementSystem.Instance.ModifyAchievementProgress("expensive_chest_10m", true, chestLootCost);
        AchievementSystem.Instance.ModifyAchievementProgress("expensive_chest_25m", true, chestLootCost);
        AchievementSystem.Instance.ModifyAchievementProgress("expensive_chest_50m", true, chestLootCost);
        AchievementSystem.Instance.ModifyAchievementProgress("expensive_chest_100m", true, chestLootCost);
    }

    public void OnGuardianSoundPlayed(int index)
    {
        bool isPickedUpEarlier = foundGuardianSounds.ContainsKey(index) ? foundGuardianSounds[index] : false;
        if (!isPickedUpEarlier)
        {
            foundGuardianSounds[index] = true;
            PlayerPrefs.SetString("Achievement_FoundGuardianSounds", GetFoundSoundsData());
            PlayerPrefs.Save();

            AchievementSystem.Instance.SetAchievementProgress("all_guardian_sounds", GetGuardianSoundsAchievementProgress());
        }
    }

    public void OnFlightStarted()
    {
        AchievementSystem.Instance.SetAchievementProgress("first_flight", 1);
    }

    public void OnQuotaCompleted(int quotaSize)
    {
        // quota count achievements
        AchievementSystem.Instance.ModifyAchievementProgress("first_quota", false, 1);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_10", false, 1);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_100", false, 1);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_1k", false, 1);

        //quota size achievements
        AchievementSystem.Instance.ModifyAchievementProgress("quota_size_1m", true, quotaSize);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_size_3m", true, quotaSize);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_size_6m", true, quotaSize);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_size_10m", true, quotaSize);
        AchievementSystem.Instance.ModifyAchievementProgress("quota_size_15m", true, quotaSize);
    }

    public void OnLevelCompleted(string locationName, bool success, int lootCost)
    {
        int locationNumber = 0;
        for (int i = 0; i < locationNames.Count; i++)
        {
            if (locationName == locationNames[i])
            {
                locationNumber = i + 1;
                if (lootCost >= requiredLootSumCollected[i])
                {
                    AchievementSystem.Instance.ModifyAchievementProgress($"location_expert_{locationNumber}", false, 1);
                }
                break;
            }
        }

        if (success)
        {
            // success flies count achievements
            AchievementSystem.Instance.ModifyAchievementProgress("success_1", false, 1);
            AchievementSystem.Instance.ModifyAchievementProgress("success_10", false, 1);
            AchievementSystem.Instance.ModifyAchievementProgress("success_100", false, 1);
            AchievementSystem.Instance.ModifyAchievementProgress("success_1k", false, 1);

            // loot cost achievements
            AchievementSystem.Instance.ModifyAchievementProgress("good_loot_1", true, lootCost);
            AchievementSystem.Instance.ModifyAchievementProgress("good_loot_5", true, lootCost);
            AchievementSystem.Instance.ModifyAchievementProgress("good_loot_10", true, lootCost);
            AchievementSystem.Instance.ModifyAchievementProgress("good_loot_20", true, lootCost);
        }
        else
        {
            // fail achievements
            AchievementSystem.Instance.ModifyAchievementProgress("fails_1", false, 1);
            AchievementSystem.Instance.ModifyAchievementProgress("fails_5", false, 1);
            AchievementSystem.Instance.ModifyAchievementProgress("fails_50", false, 1);
            AchievementSystem.Instance.ModifyAchievementProgress("fails_500", false, 1);
        }
    }

    public void OnEquipmentLevelChanged(string equipmentName, int level)
    {
        switch (equipmentName)
        {
            case "backpack":
                break;
            case "programmator":
                break;
            case "scanner":
                break;
            case "flashlight":
                break;
            case "jewelry_table":
                break;
            default: // такой ачивки нет
                return;
        }
        AchievementSystem.Instance.ModifyAchievementProgress($"perfect_{equipmentName}", true, level);
    }

    int GetLootAchievementProgress(string achID)
    {
        List<int> checkedItems = null;
        switch (achID)
        {
            case "collectionaire_books_rare":
                checkedItems = achievementDataCollectionaireBooksRare;
                break;
            case "collectionaire_books_collectional":
                checkedItems = achievementDataCollectionaireBooksCollectional;
                break;
            case "collectionaire_stones":
                checkedItems = achievementDataCollectionaireStones;
                break;
            case "collectionaire_money":
                checkedItems = achievementDataCollectionaireMoney;
                break;
            case "collectionaire_all":
                break;
            default:
                return -1;
        }

        int res = 0;
        if (checkedItems == null)
        {
            foreach (bool found in foundItems.Values)
            {
                res += found ? 1 : 0;
            }
        }
        else
        {
            foreach (int foundKey in foundItems.Keys)
            {
                if (checkedItems.Contains(foundKey))
                {
                    res += foundItems[foundKey] ? 1 : 0;
                }
            }
        }

        return res;
    }

    int GetGuardianSoundsAchievementProgress()
    {
        int res = 0;
        foreach (bool found in foundGuardianSounds.Values)
        {
            res += found ? 1 : 0;            
        }

        return res;
    }

    string GetFoundItemsData()
    {
        string res = "";
        for (int i = 1; i <= lootCategoryManager.lootCategories.Count; i++)
        {
            res += foundItems.ContainsKey(i) ? (foundItems[i] ? "1" : "0") : "0";
        }
        return res;
    }

    string GetFoundSoundsData()
    {
        string res = "";
        for (int i = 1; i <= guardianSoundsCount; i++)
        {
            res += foundGuardianSounds.ContainsKey(i) ? (foundGuardianSounds[i] ? "1" : "0") : "0";
        }
        return res;
    }

    public void EraseAchievementData()
    {
        for (int i = 0; i < AchievementSystem.Instance.GetAchievementCount(); i++)
        {
            PlayerPrefs.DeleteKey($"Achievement_{AchievementSystem.Instance.GetAchievementByIndex(i).id}_Progress");
        }
        PlayerPrefs.DeleteKey("Achievement_FoundItems");
        PlayerPrefs.DeleteKey("Achievement_FoundGuardianSounds");

        ReadFoundItemsData();
        ReadFoundGuardianSoundsData();
    }
}
