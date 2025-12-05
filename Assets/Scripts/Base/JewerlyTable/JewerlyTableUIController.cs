using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JewerlyTableUIController : UIWindowCameraTransitioning
{
    [Header("Input")]
    [SerializeField] PlayerInput playerInput;
    [Header("Stone types")]
    [SerializeField] List<string> stoneTypes = new ();
    [Header("UI elements")]
    [SerializeField] TMP_Dropdown recipeDropdown;
    [SerializeField] TMP_Dropdown stoneTypeDropdown;
    [SerializeField] GameObject stoneTypeHint;
    [Space(10)]
    [SerializeField] Button swapButton;
    [SerializeField] TMP_Text swapButtonText;
    [Header("Craft window")]
    [SerializeField] GameObject craftWindow;
    [Space(10)]
    [SerializeField] GameObject notFoundCraft;
    [Space(10)]
    [SerializeField] GameObject foundCraft;
    [SerializeField] TMP_Text requiredMaterialsText;
    [SerializeField] TMP_Text outputVariantsText;
    [Space(10)]
    [SerializeField] Button craftButton;
    [SerializeField] TMP_Text craftResultText;
    [SerializeField] Animator craftResultAnimator;
    [Header("Upgrade window")]
    [SerializeField] GameObject upgradeWindow;
    [Space(10)]
    [SerializeField] TMP_Text currentLevel;
    [SerializeField] TMP_Text upgradeCost;
    [SerializeField] TMP_Text yourBalance;
    [Space(10)]
    [SerializeField] TMP_Text upgradeEffectTitle;
    [SerializeField] TMP_Text upgradeEffectText;
    [Space(10)]
    [SerializeField] Button upgradeButton;
    [Header("Links")]
    [SerializeField] JewerlyTable jewerlyTable;

    int currentRecipe = 1, currentStoneType = 0;
    JewerlyTableCraft jewerlyTableCraft;
    int currentScreen = 0;

    protected override void UpdateCurrentInputMap()
    {
        if (visible)
        {
            InputActionMapSwitcher.Instance.SwitchMap("JewerlyTableUI");
        }
        else
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
        }
    }

    void SetScreen(int screen)
    {
        currentScreen = screen;
        craftWindow.SetActive(currentScreen == 0);
        upgradeWindow.SetActive(currentScreen == 1);

        swapButtonText.text = (currentScreen == 0) ? "Улучшения" : "Назад";
    }

    protected override void ChangeToMainMenu()
    {
        /*
        currentRecipe = 1;
        currentStoneType = 0;

        recipeDropdown.value = 0;
        stoneTypeDropdown.value = 0;
        */

        SetScreen(0);

        UpdateLayout();
    }

    protected override void OnClosed()
    {
        recipeDropdown.interactable = true;
        stoneTypeDropdown.interactable = true;
    }

    public void CloseWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "JewerlyTableUI")
        {
            recipeDropdown.interactable = false;
            stoneTypeDropdown.interactable = false;
            HideWindow();
        }   
    }

    public void UpdateLayout()
    {
        int recipe = currentRecipe;
        int stoneType = currentStoneType;
        string craftName = "";
        bool showStoneType = recipe >= 1 && recipe <= 7;

        if (showStoneType)
        {
            craftName = $"{stoneTypes[stoneType]}{recipe}";
        }
        else if (recipe == 8)
        {
            craftName = "Rubik";
        }

        //update dropdowns
        stoneTypeDropdown.interactable = showStoneType;
        stoneTypeDropdown.gameObject.SetActive(showStoneType);
        stoneTypeHint.SetActive(showStoneType);

        Debug.Log(craftName);

        jewerlyTableCraft = JewerlyTableCraftManager.Instance.GetCraftByName(craftName);

        if (currentScreen == 0)
        {
            foundCraft.SetActive(jewerlyTableCraft != null);
            notFoundCraft.SetActive(jewerlyTableCraft == null);

            if (jewerlyTableCraft != null)
            {
                bool canCraft = true;
                // build required materials
                string reqMaterials = "";
                List<string> reqMaterialsList = new();
                Dictionary<int, int> items = InventorySystem.Instance.GetItems();
                for (int i = 0; i < jewerlyTableCraft.requiredItems.Count; i++)
                {
                    LootCategory lc = jewerlyTableCraft.requiredItems[i];
                    int countInInventory = (items.ContainsKey(lc.id)) ? items[lc.id] : 0;
                    int requiredCount = jewerlyTableCraft.requiredItemsCount[i];

                    if (canCraft && countInInventory < requiredCount) canCraft = false;
                    string reqMaterialEntry = (countInInventory >= requiredCount) ? $"- {lc.lootName}: {countInInventory} / {requiredCount} шт." : $"- {lc.lootName}: <color=#ff0000>{countInInventory}</color> / {requiredCount} шт.";
                    reqMaterialsList.Add(reqMaterialEntry);
                }

                reqMaterials = string.Join("\n", reqMaterialsList);

                requiredMaterialsText.text = reqMaterials;

                //build output variants
                string outputVariants = "";
                List<string> outputVariantsList = new();
                // calculate total weight
                int totalWeight = 0;
                int tableLevel = jewerlyTable.GetLevel();

                for (int i = 0; i < jewerlyTableCraft.outputVariants.Count; i++)
                {
                    totalWeight += (int)jewerlyTableCraft.outputVariants[i].weight.FirstOrDefault((x) => { return x.level == tableLevel; }).value;
                }

                for (int i = 0; i < jewerlyTableCraft.outputVariants.Count; i++)
                {
                    int weight = (int)jewerlyTableCraft.outputVariants[i].weight.FirstOrDefault((x) => { return x.level == tableLevel; }).value;
                    float ratio = (float)weight / totalWeight;

                    string chanceText = $"{i + 1}. <color=#{((i == 0) ? "00ff00" : "ff0000")}>С вероятностью {NumberFormatter.FormatNumber(ratio * 100, 1)}%:</color>";

                    List<string> outputs = new();

                    for (int j = 0; j < jewerlyTableCraft.outputVariants[i].outputItems.Count; j++)
                    {
                        LootCategory lc = jewerlyTableCraft.outputVariants[i].outputItems[j];
                        int min = jewerlyTableCraft.outputVariants[i].outputItemsAmount[j].min;
                        int max = jewerlyTableCraft.outputVariants[i].outputItemsAmount[j].max;
                        outputs.Add($"- {lc.lootName} - {((min != max) ? $"{min}-{max}" : $"{min}")} шт.");
                    }

                    outputVariantsList.Add($"{chanceText}\n{((outputs.Count > 0) ? string.Join("\n", outputs) : "ничего")}");
                }

                outputVariants = string.Join("\n\n", outputVariantsList);

                outputVariantsText.text = outputVariants;

                craftButton.interactable = canCraft;
            }
        }
        else if (currentScreen == 1)
        {
            int currentLevel = jewerlyTable.GetLevel(), maxLevel = jewerlyTable.GetMaxLevel();
            int balance = PlayerWallet.Instance.GetMoney();

            upgradeButton.gameObject.SetActive(currentLevel < maxLevel);
            upgradeEffectTitle.gameObject.SetActive(currentLevel < maxLevel);
            upgradeEffectText.gameObject.SetActive(currentLevel < maxLevel);

            this.currentLevel.text = $"Текущий уровень: {currentLevel} / {maxLevel}";
            yourBalance.text = $"Ваш баланс: {NumberFormatter.FormatNumberWithGrouping(balance)} UMU";
            if (currentLevel == maxLevel)
            {
                upgradeCost.text = $"<color=#ffff00>Уже максимально улучшен</color>";
                
            }
            else if (currentLevel < maxLevel)
            {
                int upgradeCost = jewerlyTable.GetUpgradeCost(currentLevel + 1);
                bool canAfford = balance >= upgradeCost;
                this.upgradeCost.text = $"Стоимость улучшения: <color=#{(canAfford ? "ffffff" : "ff0000")}>{NumberFormatter.FormatNumberWithGrouping(upgradeCost)}</color> UMU";

                //build output variants
                string outputVariants = "";
                List<string> outputVariantsList = new();
                // calculate total weights
                int currentTotalWeight = 0;
                int nextTotalWeight = 0;

                for (int i = 0; i < jewerlyTableCraft.outputVariants.Count; i++)
                {
                    currentTotalWeight += (int)jewerlyTableCraft.outputVariants[i].weight.FirstOrDefault((x) => { return x.level == currentLevel; }).value;
                    nextTotalWeight += (int)jewerlyTableCraft.outputVariants[i].weight.FirstOrDefault((x) => { return x.level == currentLevel + 1; }).value;
                }

                for (int i = 0; i < jewerlyTableCraft.outputVariants.Count; i++)
                {
                    int currentWeight = (int)jewerlyTableCraft.outputVariants[i].weight.FirstOrDefault((x) => { return x.level == currentLevel; }).value;
                    float currentRatio = (float)currentWeight / currentTotalWeight;

                    int nextWeight = (int)jewerlyTableCraft.outputVariants[i].weight.FirstOrDefault((x) => { return x.level == currentLevel + 1; }).value;
                    float nextRatio = (float)nextWeight / nextTotalWeight;

                    float diff = nextRatio - currentRatio;
                    char sign = (diff > 0) ? '+': '-';
                    string color = (diff > 0 ^ i == 0) ? "ff0000" : "00ff00";
                    string diffText = (diff != 0) ? $" <color=#{color}>({sign}{NumberFormatter.FormatNumber(Mathf.Abs(diff * 100), 1)}%)</color>" : "";

                    string chanceText = $"{i + 1}. С вероятностью {NumberFormatter.FormatNumber(nextRatio * 100, 1)}%{diffText}:";

                    List<string> outputs = new();

                    for (int j = 0; j < jewerlyTableCraft.outputVariants[i].outputItems.Count; j++)
                    {
                        LootCategory lc = jewerlyTableCraft.outputVariants[i].outputItems[j];
                        int min = jewerlyTableCraft.outputVariants[i].outputItemsAmount[j].min;
                        int max = jewerlyTableCraft.outputVariants[i].outputItemsAmount[j].max;
                        outputs.Add($"- {lc.lootName} - {((min != max) ? $"{min}-{max}" : $"{min}")} шт.");
                    }

                    outputVariantsList.Add($"{chanceText}\n{((outputs.Count > 0) ? string.Join("\n", outputs) : "ничего")}");
                }

                outputVariants = string.Join("\n\n", outputVariantsList);

                upgradeEffectText.text = outputVariants;

                upgradeButton.interactable = canAfford;
            }
        }
    }

    public void OnRecipeChanged(int newRecipe)
    {
        currentRecipe = newRecipe + 1;
        UpdateLayout();
    }

    public void OnStoneTypeChanged(int newStoneType)
    {
        currentStoneType = newStoneType;
        UpdateLayout();
    }

    public void OnCreateButtonPressed()
    {
        if (jewerlyTableCraft != null)
            jewerlyTable.ExecuteCraft(jewerlyTableCraft);
    }

    public void ShowCraftResult(int result)
    {
        craftResultText.text = $"Результат: №{result+1}";
        craftResultAnimator.SetTrigger("Show");
    }

    public void OnUpgradeButtonPressed()
    {
        jewerlyTable.Upgrade();
    }

    public void SwapScreen()
    {
        if (currentScreen == 0)
        {
            SetScreen(1);
        }
        else 
        { 
            SetScreen(0);
        }

        UpdateLayout();
    }
}
