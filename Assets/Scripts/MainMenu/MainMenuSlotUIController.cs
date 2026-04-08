using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSlotUIController : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] SaveManager saveManager;
    [Space]
    [SerializeField] List<SlotButtonController> buttons = new();
    [SerializeField] Button startGameButton;
    [SerializeField] Image startGameButtonBackground;
    [Space(10)]
    [Header("Colors")]
    [SerializeField] Color slotButtonColor = new Color(.75f, .75f, .75f, .5f);
    [SerializeField] Color selectedSlotButtonColor = new Color(.75f, .75f, .0f, .5f);
    [Space]
    [SerializeField] Color startGameColor = new Color(.0f, .75f, .0f, .5f);
    [SerializeField] Color startGameNoSelectedSlotColor = new Color(.75f, .75f, .75f, .5f);
    int selectedSlot = -1;

    private void Start()
    {
        RefreshData();
    }

    public void RefreshData()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int slot = i + 1;
            bool hasFile, checksum, version;
            GameData data = saveManager.LoadData(slot, out hasFile, out checksum, out version);
            Debug.Log($"{data}, {checksum}, {version}");
            int quota = 0, collected = 0, daysLeft = 0, balance = 0;
            bool hasSave = false, isValid = false, hasQuota = false;
            if (data != null)
            {
                hasSave = true;
                isValid = checksum && version;

                if (data.save.quotaData.currentOrder.hasValue && data.save.quotaData.currentOrder.value.clientTypeID != -1)
                {
                    quota = data.save.quotaData.currentOrder.value.required;
                    collected = data.save.quotaData.collected;
                    if (collected < quota)
                    {
                        hasQuota = true;
                    }

                    daysLeft = data.save.quotaData.daysLeft;
                    balance = data.save.playerData.money;
                }
            }
            else
            {
                hasSave = hasFile;
                
                if (!version)
                {
                    buttons[i].invalidSaveReasonText = "Несовместимая версия игры";
                }
                else if (!checksum)
                {
                    buttons[i].invalidSaveReasonText = "Файл поврежден";
                }
            }
            buttons[i].hasSave = hasSave;
            buttons[i].saveIsValid = isValid;
            buttons[i].hasQuota = hasQuota;
            buttons[i].Init(slot, selectedSlotButtonColor, slotButtonColor,
                quota, collected, daysLeft, balance);
        }
    }
    public void SelectSlot(int selected)
    {
        selectedSlot = selected;
        if (selectedSlot != -1)
        {
            PlayerPrefs.SetInt("saveSlot", buttons[selectedSlot].GetNumber());
            PlayerPrefs.Save();
        }
        UpdateUI();
    }

    public int GetSelectedSlot() => selectedSlot;

    void UpdateUI()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            button.SetSelected(selectedSlot == i);
            button.UpdateUI();
        }

        startGameButtonBackground.color = selectedSlot != -1 ? startGameColor: startGameNoSelectedSlotColor;
    }
}
