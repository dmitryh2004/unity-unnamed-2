using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string inventory; // Сохраняем JSON как строку
    public int inventoryLevel;
    public int virusLevel;
    public int predictorLevel;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] string saveName = "GameData.json";

    public void SaveData()
    {
        string inventoryJson = InventorySystem.Instance.GetInventoryDataJson();
        int inventoryLevel = InventorySystem.Instance.GetLevel();
        int virusLevel = VirusController.Instance.GetLevel();
        int predictorLevel = PlayerLootPredictor.Instance.GetLevel();

        GameData data = new GameData
        {
            inventory = inventoryJson,
            inventoryLevel = inventoryLevel,
            virusLevel = virusLevel,
            predictorLevel = predictorLevel
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
