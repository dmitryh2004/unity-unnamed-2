using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    [SerializeField] bool isLevel = true;
    [SerializeField] LevelGenerator generator;
    [SerializeField] SaveManager saveManager;
    [SerializeField] StartStatsController startStatsController;
    [SerializeField] GameOverScreenController gameOverScreenController;
    [SerializeField] GameObject inventoryUI, hackUI;
    [SerializeField] TraderObject trader;

    [TextArea(5, 10)]
    [SerializeField] List<string> gameOverReasons = new();
    bool isGameOver = false;
    int slot = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        slot = PlayerPrefs.HasKey("saveSlot") ? PlayerPrefs.GetInt("saveSlot") : 1;
    }

    private void Start()
    {
        bool hasFile = false, validationResult = false, version = false;
        bool traderGenerateOrders = false;
        GameData gameData = saveManager.LoadData(slot, out hasFile, out validationResult, out version);
        if (gameData != null)
        {
            InventorySystem.Instance.SetLevel(gameData.save.playerData.inventoryLevel);
            InventorySystem.Instance.SetItemsFromJson(gameData.save.playerData.inventory);

            if (GlobalAdaptiveDifficultyManager.Instance != null)
            {
                GlobalAdaptiveDifficultyManager.Instance.SetLocationData(gameData.save.adaptiveDifficulty);
            }
            else if (AdaptiveDifficultyManager.Instance != null)
            {
                string sceneName = AdaptiveDifficultyManager.Instance.LocationName;
                AD_LocationDifficulty locationDifficulty = gameData.save.adaptiveDifficulty.locations.FirstOrDefault((x) => x.locationName == sceneName);
                AdaptiveDifficultyManager.Instance.SetAlertnessDegree(locationDifficulty.alertness);
                AdaptiveDifficultyManager.Instance.SetForgettingDegree(locationDifficulty.forgetting);
                AdaptiveDifficultyManager.Instance.SetRoomWeights(locationDifficulty.weights);
            }

            if (VirusController.Instance != null)
                VirusController.Instance.SetLevel(gameData.save.playerData.virusLevel);
            if (PlayerFlashlight.Instance != null)
                PlayerFlashlight.Instance.SetLevel(gameData.save.playerData.flashlightLevel);
            if (PlayerScanner.Instance != null)
                PlayerScanner.Instance.SetLevel(gameData.save.playerData.predictorLevel);
            if (Chest.Instance != null)
                Chest.Instance.SetItemsFromJson(gameData.save.baseData.chest);
            if (SpaceshipController.Instance != null)
                SpaceshipController.Instance.GetPanelController().SetCurrentComplexIndex(gameData.save.baseData.currentComplexIndex);
            if (JewerlyTable.Instance != null)
                JewerlyTable.Instance.SetLevel(Mathf.Clamp(gameData.save.baseData.jewerlyTableLevel, 1, JewerlyTable.Instance.GetMaxLevel()));
            if (trader != null)
                traderGenerateOrders = !(gameData.save.generatedOrders.order1.hasValue && gameData.save.generatedOrders.order2.hasValue || gameData.save.generatedOrders.order3.hasValue);

            if (QuotaSystem.Instance != null)
            {
                QuotaSystem.Instance.SetCollected(gameData.save.quotaData.collected);
                QuotaSystem.Instance.SetDaysLeft(gameData.save.quotaData.daysLeft);

                NullableOrderData orderData = gameData.save.quotaData.currentOrder;
                if (orderData.hasValue)
                {
                    ClientType ct = ClientTypeManager.Instance.GetClientType(orderData.value.clientTypeID);
                    Debug.Log("Client type: " + ct);

                    Order order = new ();
                    order.SetClientType(ct);
                    order.SetRequired(orderData.value.required);
                    order.SetMultiplier(orderData.value.multiplier);

                    QuotaSystem.Instance.SetRequired(orderData.value.required);
                    QuotaSystem.Instance.SetMultiplier(orderData.value.multiplier);
                    QuotaSystem.Instance.SetOrder(order);
                }
                else
                {
                    QuotaSystem.Instance.SetOrder(null);
                }

                QuotaSystem.Instance.UpdateUI();
            }

            PlayerWallet.Instance.SetMoney(gameData.save.playerData.money);
        }
        else
        {
            if (!(hasFile && validationResult && version))
            {
                Debug.LogWarning("Validation failed: applying default values");
                // add message box for the player
            }

            InventorySystem.Instance.SetLevel(1);

            if (VirusController.Instance != null)
                VirusController.Instance.SetLevel(1);
            if (PlayerFlashlight.Instance != null)
                PlayerFlashlight.Instance.SetLevel(1);
            if (PlayerScanner.Instance != null)
                PlayerScanner.Instance.SetLevel(0);
            if (QuotaSystem.Instance != null)
            {
                QuotaSystem.Instance.SetOrder(null);
                QuotaSystem.Instance.UpdateUI();
            }
            traderGenerateOrders = true;
        }

        if (trader != null)
        {
            trader.Init();
            if (traderGenerateOrders)
                trader.GenerateOrders();
            else
                trader.SetGeneratedOrders(new OrderData[] {gameData.save.generatedOrders.order1.value, gameData.save.generatedOrders.order2.value, gameData.save.generatedOrders.order3.value});
        }

        if (isLevel)
        {
            generator.Generate();
            Debug.Log("Level generated");
            StatisticCollector.Instance.TotalLootCost = generator.GetGeneratedLootSum();
            startStatsController.ShowStatsWindow();
        }
    }

    public void SaveGame(bool showMessage = false)
    {
        saveManager.SaveData(slot: slot, showMessage: showMessage);
    }

    public void GameOver(int reasonCode)
    {
        if (isGameOver) return;
        isGameOver = true;

        // calculate alertness increase
        if (AdaptiveDifficultyManager.Instance != null)
        {
            int newAlertness = AdaptiveDifficultyManager.Instance.AlertnessDegree;
            if (reasonCode != 0)
            {
                newAlertness = 5;
            }
            else
            {
                if (AlarmController.Instance.GetAlarmState())
                    newAlertness += 1;
            }
            newAlertness = Mathf.Clamp(newAlertness, -1, 5);
            AdaptiveDifficultyManager.Instance.SetAlertnessDegree(newAlertness);
        }

        GuardianManager.Instance.StopGuardians(); //deactivate all guardians

        InputActionMapSwitcher.Instance.DisableAllMaps(); // disable input
        InputActionMapSwitcher.Instance.ShowCursor();

        if (AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StopAlarm(); // stop alarm

        StatisticCollector.Instance.CollectedLootCost = InventorySystem.Instance.GetTotalCost(); // todo: replace it

        if (reasonCode != 0) // clear inventory if defeat
        {
            InventorySystem.Instance.RemoveAllItems();
        }

        // close all windows
        if (inventoryUI != null)
            inventoryUI.GetComponent<Animator>().SetBool("visible", false);
        if (hackUI != null)
            hackUI.GetComponent<Animator>().SetBool("visible", false);

        AchievementActionTracker.Instance?.OnLevelCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, reasonCode == 0, StatisticCollector.Instance.CollectedLootCost);

        generator.UpdateRoomWeights();
        saveManager.SaveData(slot, showMessage: true);

        gameOverScreenController.ShowGameOverWindow(reasonCode == 0, gameOverReasons[reasonCode]);
    }

    public void BaseGameOver()
    {
        saveManager.SaveData(slot);
        gameOverScreenController.ShowGameOverWindow(true, gameOverReasons[0], false);
    }

    public void DeleteSave()
    {
        saveManager.ClearSave(slot);
    }
}
