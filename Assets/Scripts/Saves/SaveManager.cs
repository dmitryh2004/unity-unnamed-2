using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public string version = "n/a";
    public Player playerData;
    public Base baseData;
    public Quota quotaData;
    public GeneratedOrders generatedOrders;
    public AD_Locations adaptiveDifficulty;

    public Save()
    {
        playerData = new Player();
        baseData = new Base();
        quotaData = new Quota();
        generatedOrders = new GeneratedOrders();
        adaptiveDifficulty = new AD_Locations();
    }
}

[System.Serializable]
public class Player
{
    public string inventory = "";
    public int inventoryLevel = 1;
    public int virusLevel = 1;
    public int flashlightLevel = 1;
    public int predictorLevel = 0;
    public int money = 0;
}

[System.Serializable]
public class Base
{
    public string chest = "";
    public int currentComplexIndex = 0;
    public int jewerlyTableLevel = 1;
}

[System.Serializable]
public class Quota
{
    public NullableOrderData currentOrder;
    public int collected = 0;
    public int daysLeft = 3;
    public Quota()
    {
        currentOrder = new NullableOrderData(null);
        collected = 0;
        daysLeft = 3;
    }
}

[System.Serializable]
public class NullableOrderData
{
    public OrderData value;
    public bool hasValue;   // Флаг наличия значения

    public NullableOrderData(OrderData data)
    {
        value = data;
        hasValue = data != null;
    }

    public OrderData GetValueOrDefault()
    {
        return hasValue ? value : null;
    }
}
[System.Serializable]
public class OrderData
{
    public float multiplier = 1.0f;
    public int required = 100000;
    public int clientTypeID = 0;
}

[System.Serializable]
public class GeneratedOrders
{
    public NullableOrderData order1, order2, order3;
}

[System.Serializable]
public class AD_RoomWeight
{
    public Vector3 roomPosition;
    public float weight;
}

[System.Serializable]
public class AD_LocationDifficulty
{
    public string locationName;
    public int forgetting = 5;
    public int alertness = -1;
    public List<AD_RoomWeight> weights = new();
}

[System.Serializable]
public class AD_Locations
{
    public List<AD_LocationDifficulty> locations = new();
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

#if UNITY_EDITOR
        Debug.LogWarning($"Correct checksum: {calculatedChecksum}");
#endif

        // Сравниваем вычисленный хеш с переданным.
        // Используем StringComparison.OrdinalIgnoreCase для нечувствительного к регистру сравнения,
        // так как хеши обычно не чувствительны к регистру, но CalculateChecksum возвращает в нижнем.
        return string.Equals(calculatedChecksum, checksum, StringComparison.OrdinalIgnoreCase);
    }
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] string saveName = "GameData";
    [SerializeField] string saveVersion = "beta2";

    [Header("Adaptive difficulty")]
    [SerializeField] List<string> locationNames = new(); 
    public string SaveVersion
    {
        get
        {
            return saveVersion;
        }
    }

    [SerializeField] Animator saveMessageAnimator;

    string GetSaveName(int slot)
    {
        return $"{saveName}{slot}.json";
    }

    public void SaveData(int slot = 1, bool showMessage = false)
    {
        bool hasFile = false, validationResult = false, version = false;
        string inventoryJson = InventorySystem.Instance.GetInventoryDataJson();

        GameData loadedData = LoadData(slot, out hasFile, out validationResult, out version, false);

        string chestJson = "";
        if (Chest.Instance != null)
        {
            chestJson = Chest.Instance.GetInventoryDataJson();
        }
        else
        {
            if (loadedData != null)
            {
                string savedChest = loadedData.save.baseData.chest;
                if (savedChest != null)
                {
                    chestJson = savedChest;
                }
                else
                {
                    chestJson = "{\"items\": []}";
                }
            }
            else
            {
                chestJson = "{\"items\": []}";
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
            if (loadedData != null)
            {
                virusLevel = loadedData.save.playerData.virusLevel;
            }
        }

        int flashlightLevel = 1;
        if (PlayerFlashlight.Instance != null)
        {
            flashlightLevel = PlayerFlashlight.Instance.GetLevel();
        }
        else
        {
            if (loadedData != null)
            {
                flashlightLevel = loadedData.save.playerData.flashlightLevel;
            }
        }

        int predictorLevel = 0;
        if (PlayerScanner.Instance != null)
        {
            predictorLevel = PlayerScanner.Instance.GetLevel();
        }
        else
        {
            if (loadedData != null)
            {
                predictorLevel = loadedData.save.playerData.predictorLevel;
            }
        }

        int currentComplexIndex = 0;
        if (SpaceshipController.Instance != null)
        {
            currentComplexIndex = SpaceshipController.Instance.GetPanelController().GetCurrentComplexIndex();
        }
        else
        {
            if (loadedData != null)
            {
                currentComplexIndex = loadedData.save.baseData.currentComplexIndex;
            }
        }

        int jewerlyTableLevel = 1;
        if (JewerlyTable.Instance != null)
        {
            jewerlyTableLevel = JewerlyTable.Instance.GetLevel();
        }
        else
        {
            if (loadedData != null)
            {
                jewerlyTableLevel = loadedData.save.baseData.jewerlyTableLevel;
            }
        }

        int money = 0;
        if (PlayerWallet.Instance != null)
        {
            money = PlayerWallet.Instance.GetMoney();
        }
        else
        {
            if (loadedData != null)
            {
                money = loadedData.save.playerData.money;
            }
        }

        Player playerData = new Player
        {
            inventory = inventoryJson,
            inventoryLevel = inventoryLevel,
            virusLevel = virusLevel,
            predictorLevel = predictorLevel,
            flashlightLevel = flashlightLevel,
            money = money
        };

        Base baseData = new Base
        {
            chest = chestJson,
            currentComplexIndex = currentComplexIndex,
            jewerlyTableLevel = jewerlyTableLevel
        };

        NullableOrderData currentOrder = QuotaSystem.Instance.HasOrder()
        ? new NullableOrderData(new OrderData
        {
            multiplier = QuotaSystem.Instance.GetMultiplier(),
            required = QuotaSystem.Instance.GetRequired(),
            clientTypeID = QuotaSystem.Instance.GetClientTypeID()
        })
        : new NullableOrderData(null);

        Debug.Log($"Save manager: currentOrder = {(!currentOrder.hasValue ? "null" : $"[mul={currentOrder.value?.multiplier}, req={currentOrder.value?.required}, ct={currentOrder.value?.clientTypeID}")}]");

        Quota quotaData = new Quota {
            currentOrder = currentOrder,
            collected = QuotaSystem.Instance.GetCollected(),
            daysLeft = QuotaSystem.Instance.GetDaysLeft(),
        };

        GeneratedOrders generatedOrders = new GeneratedOrders {
            order1 = new NullableOrderData(null),
            order2 = new NullableOrderData(null),
            order3 = new NullableOrderData(null)
        };

        if (TraderObject.Instance != null)
        {
            OrderData[] generatedOrdersData = TraderObject.Instance.GetGeneratedOrdersData();
            generatedOrders = new GeneratedOrders
            {
                order1 = new NullableOrderData(generatedOrdersData[0]),
                order2 = new NullableOrderData(generatedOrdersData[1]),
                order3 = new NullableOrderData(generatedOrdersData[2])
            };
        }
        
        // adaptive difficulty
        AD_Locations adaptiveDifficulty = new();
        for (int i = 0; i < locationNames.Count; i++)
        {
            if (GlobalAdaptiveDifficultyManager.Instance != null) // if we are in base
            {
                // check for data in GADM
                AD_LocationDifficulty gadmLocationDifficulty = GlobalAdaptiveDifficultyManager.Instance.LocationsData.locations.Find((x) => x.locationName == locationNames[i]);
                if (gadmLocationDifficulty != null)
                {
                    adaptiveDifficulty.locations.Add(gadmLocationDifficulty);
                    continue;
                }
            }
            else if (AdaptiveDifficultyManager.Instance != null) // if we are on one of locations, replace its data
            {
                string locationName = AdaptiveDifficultyManager.Instance.LocationName;
                if (locationName == locationNames[i])
                {
                    adaptiveDifficulty.locations.Add(new AD_LocationDifficulty
                    {
                        locationName = locationNames[i],
                        alertness = AdaptiveDifficultyManager.Instance.AlertnessDegree,
                        forgetting = AdaptiveDifficultyManager.Instance.ForgettingDegree,
                        weights = AdaptiveDifficultyManager.Instance.RoomWeights
                    });
                    continue;
                }
            }
            
            AD_LocationDifficulty loadedLocationDifficulty = loadedData?.save.adaptiveDifficulty.locations.Find((x) => x.locationName == locationNames[i]); // find data in loaded save
            if (loadedLocationDifficulty != null)
            {
                adaptiveDifficulty.locations.Add(loadedLocationDifficulty);
            }
            else // add default data if not found anywhere
            {
                adaptiveDifficulty.locations.Add(new AD_LocationDifficulty { locationName = locationNames[i], alertness = -1, forgetting = 5, weights = new () });
            }
        }

        Save save = new Save
        {
            playerData = playerData,
            baseData = baseData,
            quotaData = quotaData,
            version = saveVersion,
            generatedOrders = generatedOrders,
            adaptiveDifficulty = adaptiveDifficulty
        };

        string checksum = SaveChecksumCalculator.CalculateChecksum(save);

        GameData data = new GameData
        {
            save = save,
            checksum = checksum
        };

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, GetSaveName(slot));
        File.WriteAllText(path, json);

        Debug.Log($"Данные сохранены в {path}");

        if (showMessage && saveMessageAnimator != null)
            saveMessageAnimator.SetTrigger("show");
    }

    public GameData LoadData(int slot, out bool hasFile, out bool checksumCorrect, out bool versionCorrect, bool validateChecksum = true, bool checkVersion = true)
    {
        string path = Path.Combine(Application.persistentDataPath, GetSaveName(slot));

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Файл сохранения не найден по пути: {path}");
            hasFile = false;
            checksumCorrect = true;
            versionCorrect = true;
            return null;
        }

        hasFile = true;

        try
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"Данные загружены из {path}");

            if (checkVersion)
            {
                versionCorrect = saveVersion == data.save.version;
                if (!versionCorrect)
                {
                    Debug.LogWarning($"Validation error: versions are not equal");
                }
            }
            else
            {
                versionCorrect = true;
            }

            if (validateChecksum)
            {
                checksumCorrect = SaveChecksumCalculator.Validate(data.save, data.checksum);
                if (!checksumCorrect)
                {
                    Debug.LogWarning($"Validation error: checksums are not equal");
                }
            }
            else
            {
                checksumCorrect = true;
            }

            if (!(versionCorrect && checksumCorrect))
                return null;
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке данных: {e.Message}");
            checksumCorrect = false;
            versionCorrect = false;
            return null;
        }
    }

    public void ClearSave(int slot)
    {
        File.Delete(Path.Combine(Application.persistentDataPath, GetSaveName(slot)));
    }
}
