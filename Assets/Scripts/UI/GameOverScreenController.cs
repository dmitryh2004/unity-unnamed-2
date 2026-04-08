using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameOverScreenController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TMP_Text headerText, descText, statsText, statsDescText, exitHintText;
    string header, desc, stats, statsDesc, exitHint;

    Coroutine currentCoroutine = null;

    public void ShowGameOverWindow(bool victory, string reason, bool fill = true)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(MainCoroutine(victory, reason, fill));
    }

    private IEnumerator MainCoroutine(bool victory, string reason, bool fill)
    {
        if (fill)
        {
            header = (victory) ? "Вылет завершен" : "Неудачный вылет";
            headerText.text = "";

            desc = reason;
            descText.text = "";

            stats = statsText.text;
            statsText.text = "";

            statsDesc = statsDescText.text;
            statsDescText.text = "";

            exitHint = exitHintText.text;
            exitHintText.text = "";

            statsDesc = statsDesc.Replace("A", $"{NumberFormatter.FormatNumberWithGrouping(StatisticCollector.Instance.TotalLootCost)}")
                .Replace("B", $"{NumberFormatter.FormatNumberWithGrouping(StatisticCollector.Instance.CollectedLootCost)}")
                .Replace("C", $"{StatisticCollector.Instance.Percent.ToString("0.00", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.')}")
                .Replace("D", $"{StatisticCollector.Instance.LocksHacked}")
                .Replace("E", $"{StatisticCollector.Instance.FailedHacks}")
                .Replace("F", $"{StatisticCollector.Instance.LockedLocks}")
                .Replace("G", $"{(StatisticCollector.Instance.AlarmRaised ? "да" : "нет")}")
                .Replace("H", $"{AlarmController.Instance.GetTimerController().GetTimerText(internalUsage: false)}");
        }

        animator.SetTrigger("show");

        yield return new WaitForSeconds(5f);

        currentCoroutine = TypewriterTextShower.Instance.ShowText(headerText, header, () => {
            currentCoroutine = TypewriterTextShower.Instance.ShowText(descText, desc, () => {
                currentCoroutine = TypewriterTextShower.Instance.ShowText(statsText, stats, () => {
                    currentCoroutine = TypewriterTextShower.Instance.ShowText(statsDescText, statsDesc, () => {
                        currentCoroutine = TypewriterTextShower.Instance.ShowText(exitHintText, exitHint);
                    });
                });
            });
        });
    }
}
