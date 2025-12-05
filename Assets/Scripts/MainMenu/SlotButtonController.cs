using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotButtonController : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] MainMenuSlotUIController slotUIController;
    [SerializeField] Image background;
    [SerializeField] TMP_Text slotButtonText;
    [SerializeField] TMP_Text quotaText, collectedText, daysLeftText, balanceText;
    [SerializeField] ClearSlotButton clearSlotButton;

    [SerializeField] GameObject validSave, invalidSave;
    [SerializeField] TMP_Text invalidSaveReason;
    Color selectedColor, unselectedColor;
    int number;
    bool selected = false;
    [HideInInspector] public bool hasSave = false, saveIsValid = false, hasQuota = false;
    [HideInInspector] public string invalidSaveReasonText;
    int quota, collected, daysLeft, balance;
    public void Init(int number, Color selected, Color unselected, int quota, int collected, int daysLeft, int balance)
    {
        this.number = number;
        selectedColor = selected;
        unselectedColor = unselected;
        this.quota = quota;
        this.collected = collected;
        this.daysLeft = daysLeft;
        this.balance = balance;

        UpdateUI();
    }

    public int GetNumber() => number;
    public void SetSelected(bool selected) => this.selected = selected;

    public void UpdateUI()
    {
        slotButtonText.text = $"Слот {number}";
        background.color = (selected) ? selectedColor : unselectedColor;

        quotaText.gameObject.SetActive(hasSave && hasQuota);
        collectedText.gameObject.SetActive(hasSave && hasQuota);
        daysLeftText.gameObject.SetActive(hasSave && hasQuota);
        balanceText.gameObject.SetActive(hasSave);

        clearSlotButton.gameObject.SetActive(hasSave);

        button.interactable = !hasSave || (hasSave && saveIsValid);

        if (hasSave)
        {
            validSave.SetActive(saveIsValid);
            invalidSave.SetActive(!saveIsValid);
            if (saveIsValid)
            {
                if (hasQuota)
                {
                    quotaText.text = $"Требуется: {NumberFormatter.FormatNumberWithGrouping(quota)} UMU";
                    collectedText.text = $"Собрано: {NumberFormatter.FormatNumberWithGrouping(collected)} UMU";
                    daysLeftText.text = $"Осталось вылетов: {daysLeft}";
                }
                balanceText.text = $"Баланс: {NumberFormatter.FormatNumberWithGrouping(balance)} UMU";
            }
            else
            {
                invalidSaveReason.text = invalidSaveReasonText;
            }
        }
        else
        {
            validSave.SetActive(false);
            invalidSave.SetActive(false);
        }
    }
}
