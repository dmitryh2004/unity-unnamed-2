#if ALLOW_CHEATS
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using System;

public class CheatController : MonoBehaviour
{
    [SerializeField] LootCategoryManager lcm;
    [SerializeField] CheatUIController uiController;

    [Header("Настройки паролей")]
    [SerializeField] private float recordingMaxDuration = 30f; // ограничиваем ввод пароля 30 секундами
    [Space]
    [SerializeField] private string toggleAdaptiveDifficultyPassword = "343507";
    [SerializeField] private string spawnItemsPassword = "734343";
    [SerializeField] private string teleportToOriginPassword = "734507";
    [SerializeField] private string successLockPassword = "032166";

    public static CheatController Instance = null;

    private bool AD_disabled = false;
    public bool AD_Disabled => AD_disabled;
    private void SetADDisabled(bool value)
    {
        AD_disabled = value;
        uiController?.SetADDisabled(AD_disabled);
    }

    private List<char> currentInput = new List<char>();
    private bool isRecording = false;
    private float recordingTimer = 0f;

    private string spawnItemsSyntax = "[INSERT][password][*][lootCategoryID: int (> 0)][*][count: int (> 0, <= 20)][END]";

    private void Awake() 
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnKeyPressed(string keyName)
    {
        if (SceneManager.GetActiveScene().name == "DefeatScene") return; // Не позволяем пользоваться читами в катсцене поражения

        // 1. Начало ввода (Insert)
        if (keyName == "Insert")
        {
            StartRecording();
            return;
        }

        // 2. Конец ввода (End)
        if (keyName == "End")
        {
            FinishRecording();
            return;
        }

        // 3. Считывание цифр в процессе записи
        if (isRecording)
        {
            // Фильтруем только клавиши Numpad (н-р: "numpad1", "numpad2"...)
            if (keyName.StartsWith("Numpad") && keyName.Length > 6)
            {
                char symbol = '\0';
                if (keyName.Length == 7) 
                {
                    symbol = keyName[6]; // Берем символ после слова "numpad"
                }
                else {
                    symbol = keyName switch
                    {
                        "NumpadDivide" => '/',
                        "NumpadMultiply" => '*',
                        "NumpadMinus" => '-',
                        "NumpadPlus" => '+',
                        "NumpadPeriod" => '.', // или ',', зависит от локали
                        _ => '\0'
                    };
                }

                if (symbol != '\0') 
                {
                    currentInput.Add(symbol);
                    //Debug.Log($"Чит-панель - записан символ: {symbol}");
                }
            }
        }
    }

    private void StartRecording()
    {
        isRecording = true;
        currentInput.Clear();
        //Debug.Log("Чит-панель - запись пароля начата...");
    }

    private void FinishRecording()
    {
        if (!isRecording) return;
        
        isRecording = false;
        string result = new string(currentInput.ToArray());

        if (result == toggleAdaptiveDifficultyPassword)
        {
            SetADDisabled(!AD_disabled);
            //Debug.Log($"Команда выполнена - система динамической сложности {((AD_disabled) ? "отключена" : "включена")}");
        }
        else if (result == teleportToOriginPassword) 
        {
            TeleportPlayer();
        }
        else if (result == successLockPassword)
        {
            HackWindowController hwc = GameObject.FindFirstObjectByType<HackWindowController>();
            if (hwc.IsHacking())
            {
                hwc.SuccessLock();
            }
        }
        else if (result.StartsWith(spawnItemsPassword + "*"))
        {
            bool success = false, validSyntax = false, playerFound = false, itemFound = false;
            string[] splits = result.Split("*");
            if (splits.Length == 3) 
            {
                if (int.TryParse(splits[1], out int lcID) && int.TryParse(splits[2], out int count)) 
                {
                    if (lcID > 0 && count > 0 && count <= 20) 
                    {
                        validSyntax = true;
                        // find player
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        if (player != null) 
                        {
                            playerFound = true;
                            LootCategory lc = InventorySystem.Instance.GetLootCategoryById(lcID);
                            if (lc != null) 
                            {
                                itemFound = true;
                                for (int i = 0; i < count; i++) {
                                    GameObject.Instantiate(lc.lootPrefab, player.transform.position + Vector3.up * 1f, Quaternion.Euler(0f, 0f, 0f));
                                }
                                success = true;
                                //Debug.Log($"Команда выполнена - id={lcID}, создано объектов: {count}");
                            }
                        }
                    }
                }
            }
            
            if (!success) 
            {
                /*
                if (!validSyntax)
                    Debug.LogError($"Не удалось выполнить команду - неверный синтаксис: {spawnItemsSyntax}");
                else if (!playerFound)
                    Debug.LogError("Не удалось выполнить команду - не найден объект игрока");
                else if (!itemFound)
                    Debug.LogError($"Не удалось выполнить команду - не найден предмет с ID={splits[2]}");
                */
            }
        }
        else
        {
            //Debug.LogError($"Неверный код: {result}");
        }
    }

    void Update() 
    {
        CheckKeys();
        UpdateRecordingTimer();
    }

    private void CheckKeys()
    {
        if (Keyboard.current == null)
            return;

        if (isRecording)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                foreach (var key in Keyboard.current.allKeys)
                {
                    if (key.wasPressedThisFrame)
                    {
                        // Debug.Log($"Нажата клавиша: {key.displayName} ({key.keyCode})");
                        OnKeyPressed(key.keyCode.ToString());
                        break;
                    }
                }
            }
        }
        else
        {
            if (Keyboard.current.insertKey.wasPressedThisFrame)
            {
                OnKeyPressed(Keyboard.current.insertKey.keyCode.ToString());
            }
        }
    }

    private void UpdateRecordingTimer()
    {
        if (isRecording)
        {
            recordingTimer += Time.deltaTime;
            if (recordingTimer >= recordingMaxDuration)
            {
                isRecording = false;
                //Debug.LogWarning("Истекло время на ввод команды");
            }
        }
        else
        {
            recordingTimer = 0f;
            if (currentInput.Count > 0) currentInput.Clear();
        }
    }

    private void TeleportPlayer()
    {
        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 newPlayerPosition = new Vector3(0f, 1f, 0f);
            // Пытаемся получить Rigidbody
            if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                // 1. Обнуляем скорости, чтобы игрока не "выстрелило" по инерции после переноса
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // 2. Телепортируем через Rigidbody.position (это безопаснее для физики)
                rb.position = newPlayerPosition;
                
                // 3. Синхронизируем Transform (на всякий случай)
                player.transform.position = newPlayerPosition;
            }
            else
            {
                // Если Rigidbody нет, просто двигаем Transform
                player.transform.position = newPlayerPosition;
                //Debug.LogWarning("У игрока не найден Rigidbody, телепортирован через Transform.");
            }
        }
        else
        {
            //Debug.LogError("Не удалось выполнить команду - не найден объект игрока");
        }
    }
}
#endif