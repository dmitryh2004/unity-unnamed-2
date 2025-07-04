using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    [SerializeField] LevelGenerator generator;
    [SerializeField] SaveManager saveManager;
    [SerializeField] StartStatsController startStatsController;

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
        startStatsController.ShowStatsWindow();
    }
}
