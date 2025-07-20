using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string inventory; // Сохраняем JSON как строку
    public string chest;
    public int currentComplexIndex = 0;
    public int inventoryLevel = 1;
    public int virusLevel = 1;
    public int predictorLevel = 0;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] string saveName = "GameData.json";

    public void SaveData()
    {
        string inventoryJson = InventorySystem.Instance.GetInventoryDataJson();

        string chestJson = "";
        if (Chest.Instance != null)
        {
            chestJson = Chest.Instance.GetInventoryDataJson();
        }
        else
        {
            string savedChest = LoadData().chest;
            if (savedChest != null)
            {
                chestJson = savedChest;
            }
        }
        int inventoryLevel = InventorySystem.Instance.GetLevel();

        int virusLevel = 1;
        if (VirusController.Instance != null)
        {
            virusLevel = VirusController.Instance.GetLevel();
        }
        else
        {
            virusLevel = LoadData().virusLevel;
        }
        
        int predictorLevel = 0;
        if (PlayerLootPredictor.Instance != null)
        {
            predictorLevel = PlayerLootPredictor.Instance.GetLevel();
        }
        else
        {
            predictorLevel = LoadData().predictorLevel;
        }

        int currentComplexIndex = 0;
        if (SpaceshipController.Instance != null)
        {
            currentComplexIndex = SpaceshipController.Instance.GetPanelController().GetCurrentComplexIndex();
        }
        else
        {
            currentComplexIndex = LoadData().currentComplexIndex;
        }

        GameData data = new GameData
        {
            inventory = inventoryJson,
            chest = chestJson,
            inventoryLevel = inventoryLevel,
            virusLevel = virusLevel,
            predictorLevel = predictorLevel,
            currentComplexIndex = currentComplexIndex
        };

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, saveName);
        File.WriteAllText(path, json);

        Debug.Log($"Данные сохранены в {path}");
    }

    public GameData LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, saveName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Файл сохранения не найден по пути: {path}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"Данные загружены из {path}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при загрузке данных: {e.Message}");
            return null;
        }
    }
}
