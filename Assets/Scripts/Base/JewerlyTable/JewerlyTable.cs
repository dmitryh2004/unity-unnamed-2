using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class JewerlyTable : Interactable
{
    public static JewerlyTable Instance = null;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] JewerlyTableUIController jewerlyTableUIController;
    [SerializeField] Transform spawnPoint;
    [Range(1, 7)]
    [SerializeField] int level = 1;
    [SerializeField] int maxLevel = 7;
    [SerializeField] List<UpgradeCost> upgradeCosts = new();
    [SerializeField] List<UpgradableValue> quotaMultiplierStepBonus = new ();

    [SerializeField] JewerlyTableAudioPlayer audioPlayer;
    System.Random random = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void Interact()
    {
        jewerlyTableUIController.ShowWindow();
    }

    public int GetMaxLevel() => maxLevel;
    public int GetLevel() => level;
    public void SetLevel(int level)
    {
        this.level = level;
        AchievementActionTracker.Instance?.OnEquipmentLevelChanged("jewelry_table", level);
    }
    public int GetUpgradeCost(int level) => upgradeCosts.FirstOrDefault((x) => x.level == level).cost;
    public float GetQuotaMultiplierStepBoost() => quotaMultiplierStepBonus.FirstOrDefault((x) => x.level == level).value;
    public float GetQuotaMultiplierBoost(int level) => quotaMultiplierStepBonus.FirstOrDefault((x) => x.level == level).value;

    public void Upgrade()
    {
        if (level == maxLevel) return;
        int upgradeCost = GetUpgradeCost(GetLevel() + 1);
        int balance = PlayerWallet.Instance.GetMoney();

        if (upgradeCost <= balance)
        {
            level += 1;
            PlayerWallet.Instance.SubtractMoney(upgradeCost);
        }

        jewerlyTableUIController.UpdateLayout();
    }

    public void ExecuteCraft(JewerlyTableCraft craft)
    {
        int totalWeight = 0, rand = 0;

        foreach(JewerlyTableOutputVariant ov in craft.outputVariants)
        {
            totalWeight += (int) ov.weight.FirstOrDefault((x) => x.level == level).value;
        }

        rand = random.Next(0, totalWeight);
        int weight = 0;
        int choice = 0;

        for (int i = 0; i < craft.outputVariants.Count; i++)
        {
            weight += (int) craft.outputVariants[i].weight.FirstOrDefault((x) => x.level == level).value;
            if (weight > rand)
            {
                choice = i;
                break;
            }
        }

        for (int i = 0; i < craft.requiredItems.Count; i++)
        {
            InventorySystem.Instance.RemoveItem(craft.requiredItems[i], craft.requiredItemsCount[i]);
        }

        for (int i = 0; i < craft.outputVariants[choice].outputItems.Count; i++)
        {
            int spawnAmount = random.Next(craft.outputVariants[choice].outputItemsAmount[i].min, craft.outputVariants[choice].outputItemsAmount[i].max);
            for (int j = 0; j < spawnAmount; j++)
            {
                GameObject output = Instantiate(craft.outputVariants[choice].outputItems[i].lootPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                output.transform.localScale = Vector3.one;
            }
        }

        jewerlyTableUIController.UpdateLayout();
        jewerlyTableUIController.ShowCraftResult(choice);
        audioPlayer.PlayUseAudio();
    }
}
