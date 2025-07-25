using TMPro;
using UnityEngine;

public class QuotaUIController : MonoBehaviour
{
    [SerializeField] TMP_Text requiredText, collectedText, daysLeftText;
    [SerializeField] GameObject hasQuotaObject, noQuotaObject, quotaCompletedObject;

    public void UpdateUI()
    {
        bool hasOrder = QuotaSystem.Instance.HasOrder();
        int collected = QuotaSystem.Instance.GetCollected();
        int required = QuotaSystem.Instance.GetRequired();

        hasQuotaObject.SetActive(hasOrder && collected < required);
        noQuotaObject.SetActive(!hasOrder);
        quotaCompletedObject.SetActive(hasOrder && collected >= required);

        if (hasOrder)
        {
            requiredText.text = $"{NumberFormatter.FormatNumberWithGrouping(required)}";
            collectedText.text = $"{NumberFormatter.FormatNumberWithGrouping(collected)}";
            daysLeftText.text = $"{QuotaSystem.Instance.GetDaysLeft()}";
        }
    }
}
