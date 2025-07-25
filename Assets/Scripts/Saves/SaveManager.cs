using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string checksum = "";
    public Save save;
}

[System.Serializable]
public class Save
{
    public Player playerData;
    public Base baseData;
    public Quota quotaData;

    public Save()
    {
        playerData = new Player();
        baseData = new Base();
        quotaData = new Quota();
    }
}

[System.Serializable]
public class Player
{
    public string inventory = "";
    public int inventoryLevel = 1;
    public int virusLevel = 1;
    public int predictorLevel = 0;
    public int money = 0;
}

[System.Serializable]
public class Base
{
    public string chest = "";
    public int currentComplexIndex = 0;
}

[System.Serializable]
public class Quota
{
    public float multiplier = 1.0f;
    public int required = 100000;
    public int collected = 0;
    public int daysLeft = 3;
    public int clientTypeID = 0;
}

public static class SaveChecksumCalculator
{
    /// <summary>
    /// Вычисляет MD5-хеш объекта Save, сериализуя его в JSON.
    /// </summary>
    /// <param name="save">Объект сохранения, для которого нужно вычислить хеш.</param>
    /// <returns>Строковое представление MD5-хеша (в нижнем регистре) или null, если save равен null.</returns>
    public static string CalculateChecksum(Save save)
    {
        if (save == null)
        {
            return null;
        }

        try
        {
            // Сериализуем объект Save в JSON-строку
            string jsonString = JsonUtility.ToJson(save);

            using (MD5 md5 = MD5.Create())
            {
                // Вычисляем хеш от байтов JSON-строки (кодировка UTF8)
                byte[] inputBytes = Encoding.UTF8.GetBytes(jsonString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Преобразуем массив байтов хеша в строковое представление (шестнадцатеричное)
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" форматирует байт как два шестнадцатеричных символа
                }
                return sb.ToString();
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибок сериализации или хеширования
            Debug.LogError($"Ошибка при вычислении контрольной суммы: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Проверяет, совпадает ли переданная контрольная сумма с вычисленной для объекта Save.
    /// </summary>
    /// <param name="save">Объект сохранения для проверки.</param>
    /// <param name="checksum">Ожидаемая контрольная сумма (строка).</param>
    /// <returns>True, если контрольные суммы совпадают; False в противном случае.</returns>
    public static bool Validate(Save save, string checksum)
    {
        if (save == null || string.IsNullOrEmpty(checksum))
        {
            return false; // Невозможно проверить, если сохранение или хеш отсутствуют
        }

        string calculatedChecksum = CalculateChecksum(save);

        // Сравниваем вычисленный хеш с переданным.
        // Используем StringComparison.OrdinalIgnoreCase для нечувствительного к регистру сравнения,
        // так как хеши обычно не чувствительны к регистру, но CalculateChecksum возвращает в нижнем.
        return string.Equals(calculatedChecksum, checksum, StringComparison.OrdinalIgnoreCase);
    }
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] string saveName = "GameData.json";

    public void SaveData()
    {
        bool validationResult = false;
        string inventoryJson = InventorySystem.Instance.GetInventoryDataJson();

        string chestJson = "";
        if (Chest.Instance != null)
        {
            chestJson = Chest.Instance.GetInventoryDataJson();
        }
        else
        {
            string savedChest = LoadData(out validationResult, false).save.baseData.chest;
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
            virusLevel = LoadData(out validationResult, false).save.playerData.virusLevel;
        }
        
        int predictorLevel = 0;
        if (PlayerLootPredictor.Instance != null)
        {
            predictorLevel = PlayerLootPredictor.Instance.GetLevel();
        }
        else
        {
            predictorLevel = LoadData(out validationResult, false).save.playerData.predictorLevel;
        }

        int currentComplexIndex = 0;
        if (SpaceshipController.Instance != null)
        {
            currentComplexIndex = SpaceshipController.Instance.GetPanelController().GetCurrentComplexIndex();
        }
        else
        {
            currentComplexIndex = LoadData(out validationResult, false).save.baseData.currentComplexIndex;
        }

        int money = 0;
        if (PlayerWallet.Instance != null)
        {
            money = PlayerWallet.Instance.GetMoney();
        }
        else
        {
            money = LoadData(out validationResult, false).save.playerData.money;
        }

        Player playerData = new Player
        {
            inventory = inventoryJson,
            inventoryLevel = inventoryLevel,
            virusLevel = virusLevel,
            predictorLevel = predictorLevel,
            money = money
        };

        Base baseData = new Base
        {
            chest = chestJson,
            currentComplexIndex = currentComplexIndex
        };

        Quota quotaData = new Quota {
            multiplier = QuotaSystem.Instance.GetMultiplier(),
            required = QuotaSystem.Instance.GetRequired(),
            collected = QuotaSystem.Instance.GetCollected(),
            daysLeft = QuotaSystem.Instance.GetDaysLeft(),
            clientTypeID = QuotaSystem.Instance.GetClientTypeID()
        };

        Save save = new Save
        {
            playerData = playerData,
            baseData = baseData,
            quotaData = quotaData
        };

        string checksum = SaveChecksumCalculator.CalculateChecksum(save);

        GameData data = new GameData
        {
            save = save,
            checksum = checksum
        };

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, saveName);
        File.WriteAllText(path, json);

        Debug.Log($"Данные сохранены в {path}");
    }

    public GameData LoadData(out bool checksumCorrect, bool validateChecksum = true)
    {
        string path = Path.Combine(Application.persistentDataPath, saveName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Файл сохранения не найден по пути: {path}");
            checksumCorrect = true;
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"Данные загружены из {path}");

            if (validateChecksum)
            {
                checksumCorrect = SaveChecksumCalculator.Validate(data.save, data.checksum);
                if (checksumCorrect == false)
                {
                    Debug.LogWarning($"Validation error: checksums are not equal");
                    return null;
                }
            }
            else
            {
                checksumCorrect = true;
            }
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке данных: {e.Message}");
            checksumCorrect = false;
            return null;
        }
    }

    public void ClearSave()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, saveName));
    }
}
