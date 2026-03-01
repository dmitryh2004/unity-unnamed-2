using System.Collections.Generic;
using UnityEngine;

public class AchievementActionTracker : MonoBehaviour
{
    public static AchievementActionTracker Instance = null;
    [SerializeField] LootCategoryManager lootCategoryManager;
    [Header("Location names")]
    [SerializeField] List<string> locationNames = new ();

    [Header("Collectonaire achievements")]
    [SerializeField] List<int> achievementDataCollectionaireBooksRare;
    [SerializeField] List<int> achievementDataCollectionaireBooksCollectional;
    [SerializeField] List<int> achievementDataCollectionaireStones;
    [SerializeField] List<int> achievementDataCollectionaireMoney;
    Dictionary<int, bool> foundItems = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

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

    public void OnLevelCompleted(string locationName, bool success, int lootCost)
    {
        int locationNumber = 0;
        for (int i = 0; i < locationNames.Count; i++)
        {
            if (locationName == locationNames[i])
            {
                locationNumber = i + 1;
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
        // fail achievements
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

    string GetFoundItemsData()
    {
        string res = "";
        for (int i = 1; i <= lootCategoryManager.lootCategories.Count; i++)
        {
            res += foundItems.ContainsKey(i) ? (foundItems[i] ? "1" : "0") : "0";
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
    }
}
