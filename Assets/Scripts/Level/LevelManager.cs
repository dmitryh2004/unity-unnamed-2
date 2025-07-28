using System.Collections.Generic;
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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        bool validationResult = false;
        GameData gameData = saveManager.LoadData(out validationResult);
        if (gameData != null)
        {
            InventorySystem.Instance.SetLevel(gameData.save.playerData.inventoryLevel);
            InventorySystem.Instance.SetItemsFromJson(gameData.save.playerData.inventory);

            if (VirusController.Instance != null)
                VirusController.Instance.SetLevel(gameData.save.playerData.virusLevel);
            if (PlayerLootPredictor.Instance != null)
                PlayerLootPredictor.Instance.SetLevel(gameData.save.playerData.predictorLevel);
            if (Chest.Instance != null)
                Chest.Instance.SetItemsFromJson(gameData.save.baseData.chest);
            if (SpaceshipController.Instance != null)
                SpaceshipController.Instance.GetPanelController().SetCurrentComplexIndex(gameData.save.baseData.currentComplexIndex);

            if (QuotaSystem.Instance != null)
            {
                QuotaSystem.Instance.SetRequired(gameData.save.quotaData.required);
                QuotaSystem.Instance.SetCollected(gameData.save.quotaData.collected);
                QuotaSystem.Instance.SetDaysLeft(gameData.save.quotaData.daysLeft);
                QuotaSystem.Instance.SetMultiplier(gameData.save.quotaData.multiplier);

                ClientType ct = ClientTypeManager.Instance.GetClientType(gameData.save.quotaData.clientTypeID);
                Debug.Log("Client type: " + ct);

                if (ct != null)
                {
                    Order order = new ();
                    order.SetClientType(ct);
                    order.SetRequired(gameData.save.quotaData.required);
                    order.SetMultiplier(gameData.save.quotaData.multiplier);
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
            if (validationResult == false)
            {
                Debug.LogWarning("Validation failed: applying default values");
                // add message box for the player
            }

            InventorySystem.Instance.SetLevel(1);

            if (VirusController.Instance != null)
                VirusController.Instance.SetLevel(1);
            if (PlayerLootPredictor.Instance != null)
                PlayerLootPredictor.Instance.SetLevel(0);
        }

        if (trader != null)
        {
            trader.Init();
        }

        if (isLevel)
        {
            generator.Generate();
            StatisticCollector.Instance.TotalLootCost = generator.GetGeneratedLootSum();
            startStatsController.ShowStatsWindow();
        }
    }

    public void GameOver(int reasonCode)
    {
        GuardianManager.Instance.StopGuardians(); //deactivate all guardians

        InputActionMapSwitcher.Instance.DisableAllMaps(); // disable input
        InputActionMapSwitcher.Instance.ShowCursor();

        if (AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StopAlarm(); // stop alarm

        StatisticCollector.Instance.CollectedLootCost = InventorySystem.Instance.GetTotalCost();

        if (reasonCode != 0) // clear inventory if defeat
        {
            InventorySystem.Instance.RemoveAllItems();
        }

        // close all windows
        if (inventoryUI != null)
            inventoryUI.GetComponent<Animator>().SetBool("visible", false);
        if (hackUI != null)
            hackUI.GetComponent<Animator>().SetBool("visible", false);

        saveManager.SaveData();

        gameOverScreenController.ShowGameOverWindow(reasonCode == 0, gameOverReasons[reasonCode]);
    }

    public void BaseGameOver()
    {
        saveManager.SaveData();
        gameOverScreenController.ShowGameOverWindow(true, gameOverReasons[0], false);
    }

    public void DeleteSave()
    {
        saveManager.ClearSave();
    }
}
