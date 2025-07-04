using System.Collections;
using TMPro;
using UnityEngine;

public class DefeatScreenController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TMP_Text headerText, descText, statsText, statsDescText, exitHintText;
    string header, desc, stats, statsDesc, exitHint;

    Coroutine currentCoroutine = null;
    TypewiterAudioPlayer tap;

    private void Start()
    {
        tap = GetComponent<TypewiterAudioPlayer>();
    }

    public void ShowDefeatWindow(string reason)
    {
        StartCoroutine(MainCoroutine(reason));
    }

    private IEnumerator MainCoroutine(string reason)
    {
        header = headerText.text;
        headerText.text = "";

        desc = reason;
        descText.text = "";

        stats = statsText.text;
        statsText.text = "";

        statsDesc = statsDescText.text;
        statsDescText.text = "";

        exitHint = exitHintText.text;
        exitHintText.text = "";

        statsDesc = statsDesc.Replace("A", $"{StatisticCollector.Instance.TotalLootCost}")
            .Replace("B", $"{StatisticCollector.Instance.CollectedLootCost}")
            .Replace("C", $"{StatisticCollector.Instance.Percent}")
            .Replace("D", $"{StatisticCollector.Instance.LocksHacked}")
            .Replace("E", $"{StatisticCollector.Instance.FailedHacks}")
            .Replace("F", $"{StatisticCollector.Instance.LockedLocks}")
            .Replace("G", $"{(StatisticCollector.Instance.AlarmRaised ? "да" : "нет")}")
            .Replace("H", $"{AlarmController.Instance.GetTimerController().GetTimerText()}");

        animator.SetTrigger("show");

        yield return new WaitForSeconds(5f);

        currentCoroutine = TypewriterTextShower.Instance.ShowText(headerText, header, tap, () => {
            currentCoroutine = TypewriterTextShower.Instance.ShowText(descText, desc, tap, () => {
                currentCoroutine = TypewriterTextShower.Instance.ShowText(statsText, stats, tap, () => {
                    currentCoroutine = TypewriterTextShower.Instance.ShowText(statsDescText, statsDesc, tap, () => {
                        currentCoroutine = TypewriterTextShower.Instance.ShowText(exitHintText, exitHint, tap);
                    });
                });
            });
        });
    }
}
