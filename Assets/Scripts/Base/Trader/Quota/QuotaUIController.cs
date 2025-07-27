using TMPro;
using UnityEngine;

public class QuotaUIController : MonoBehaviour
{
    [SerializeField] TMP_Text requiredText, collectedText, daysLeftText;
    [SerializeField] GameObject hasQuotaObject, noQuotaObject, quotaCompletedObject;

    public void UpdateUI()
    {
        bool hasOrder = QuotaSystem.Instance.HasOrder();
        bool hasUncompletedOrder = QuotaSystem.Instance.HasUncompletedOrder();
        bool hasCompletedOrder = QuotaSystem.Instance.HasCompletedOrder();
        int collected = QuotaSystem.Instance.GetCollected();
        int required = QuotaSystem.Instance.GetRequired();

        hasQuotaObject.SetActive(hasUncompletedOrder);
        noQuotaObject.SetActive(!hasOrder);
        quotaCompletedObject.SetActive(hasCompletedOrder);

        if (hasOrder)
        {
            requiredText.text = $"{NumberFormatter.FormatNumberWithGrouping(required)}";
            collectedText.text = $"{NumberFormatter.FormatNumberWithGrouping(collected)}";
            daysLeftText.text = $"{QuotaSystem.Instance.GetDaysLeft()}";
        }
    }
}
