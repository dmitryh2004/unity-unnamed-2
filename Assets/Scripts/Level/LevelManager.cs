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
        Debug.Log($"Initialized:\n- inventory system: {InventorySystem.Instance != null};\n- virus: {VirusController.Instance != null};\nloot predictor: {PlayerLootPredictor.Instance != null}");
        GameData gameData = saveManager.LoadData();
        if (gameData != null)
        {
            InventorySystem.Instance.SetLevel(gameData.inventoryLevel);
            InventorySystem.Instance.SetItemsFromJson(gameData.inventory);

            if (VirusController.Instance != null)
                VirusController.Instance.SetLevel(gameData.virusLevel);
            if (PlayerLootPredictor.Instance != null)
                PlayerLootPredictor.Instance.SetLevel(gameData.predictorLevel);
            if (Chest.Instance != null)
                Chest.Instance.SetItemsFromJson(gameData.chest);
        }
        else
        {
            InventorySystem.Instance.SetLevel(1);

            if (VirusController.Instance != null)
                VirusController.Instance.SetLevel(1);
            if (PlayerLootPredictor.Instance != null)
                PlayerLootPredictor.Instance.SetLevel(0);
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

        saveManager.SaveData();

        gameOverScreenController.ShowGameOverWindow(reasonCode == 0, gameOverReasons[reasonCode]);
    }

    public void BaseGameOver()
    {
        saveManager.SaveData();
        gameOverScreenController.ShowGameOverWindow(true, gameOverReasons[0], false);
    }
}
