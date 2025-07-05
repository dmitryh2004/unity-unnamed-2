using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    [SerializeField] LevelGenerator generator;
    [SerializeField] SaveManager saveManager;
    [SerializeField] StartStatsController startStatsController;
    [SerializeField] GameOverScreenController gameOverScreenController;

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
        GameData gameData = saveManager.LoadData();
        if (gameData != null)
        {
            InventorySystem.Instance.SetLevel(gameData.inventoryLevel);
            VirusController.Instance.SetLevel(gameData.virusLevel);
            PlayerLootPredictor.Instance.SetLevel(gameData.predictorLevel);
            InventorySystem.Instance.SetItemsFromJson(gameData.inventory);
        }
        else
        {
            InventorySystem.Instance.SetLevel(1);
            VirusController.Instance.SetLevel(1);
            PlayerLootPredictor.Instance.SetLevel(0);
        }
        generator.Generate();
        StatisticCollector.Instance.TotalLootCost = generator.GetGeneratedLootSum();
        startStatsController.ShowStatsWindow();
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

        saveManager.SaveData();

        gameOverScreenController.ShowGameOverWindow(reasonCode == 0, gameOverReasons[reasonCode]);
    }
}
