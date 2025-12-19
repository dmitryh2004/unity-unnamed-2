using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUIController : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField] TMP_Text currentChargePercentText;
    [SerializeField] Animator animator;
    public void UpdateUI(float currentCharge, float maxCharge)
    {
        float ratio = currentCharge / maxCharge;

        progressBar.fillAmount = ratio;
        animator.SetFloat("power", ratio);

        currentChargePercentText.text = $"{NumberFormatter.FormatNumber(ratio * 100, minDigits: 0, maxDigits: 0)}%";
    }
}
