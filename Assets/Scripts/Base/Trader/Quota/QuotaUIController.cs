using TMPro;
using UnityEngine;

public class QuotaUIController : MonoBehaviour
{
    [SerializeField] TMP_Text requiredText, collectedText, daysLeftText;
    [SerializeField] GameObject hasQuotaObject, noQuotaObject;

    public void UpdateUI()
    {
        bool hasOrder = QuotaSystem.Instance.HasOrder();

        hasQuotaObject.SetActive(hasOrder);
        noQuotaObject.SetActive(!hasOrder);

        if (hasOrder)
        {
            requiredText.text = $"{QuotaSystem.Instance.GetRequired()}";
            collectedText.text = $"{QuotaSystem.Instance.GetCollected()}";
            daysLeftText.text = $"{QuotaSystem.Instance.GetDaysLeft()}";
        }
    }
}
