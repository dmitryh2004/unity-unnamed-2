using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StartStatsController : MonoBehaviour
{
    [SerializeField] LevelGenerator generator;
    [SerializeField] Animator animator;
    [SerializeField] TMP_Text title, estimatedCost, protectedRoomCount, securedRoomCount;
    string titleText, estimatedCostText, protectedRoomCountText, securedRoomCountText;

    Coroutine currentCoroutine = null;

    public void ShowStatsWindow()
    {
        int precision = PlayerLootPredictor.Instance.GetPrecision();
        StartCoroutine(MainCoroutine(0.5f, precision));
    }

    private IEnumerator MainCoroutine(float startDelay, int precision)
    {
        yield return new WaitForSecondsRealtime(startDelay);

        int totalCost = generator.GetGeneratedLootSum();

        titleText = "Краткая сводка";
        estimatedCostText = $"Примерная стоимость вещей:\n";

        if (precision != -1)
        {
            if (precision > 0)
            {
                int minCost = totalCost / precision * precision;
                int maxCost = (totalCost / precision + 1) * precision;
                estimatedCostText += $"{NumberFormatter.FormatNumberWithGrouping(minCost)} - {NumberFormatter.FormatNumberWithGrouping(maxCost)}";
            }
            else if (precision == 0)
            {
                estimatedCostText += $"{NumberFormatter.FormatNumberWithGrouping(totalCost)}";
            }
        }
        else
        {
            estimatedCostText += "[неизвестно]";
        }

        int protectedRooms = generator.GetProtectedRoomsCount();
        int securedRooms = generator.GetSecuredRoomsCount();

        protectedRoomCountText = $"Число защищенных комнат: {protectedRooms}";
        securedRoomCountText = $"Число комнат-тайников: {securedRooms}";

        animator.SetTrigger("show");

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSecondsRealtime(3f);

        currentCoroutine = TypewriterTextShower.Instance.ShowText(title, titleText, () => {
            currentCoroutine = TypewriterTextShower.Instance.ShowText(estimatedCost, estimatedCostText, () => {
                currentCoroutine = TypewriterTextShower.Instance.ShowText(protectedRoomCount, protectedRoomCountText, () => {
                    currentCoroutine = TypewriterTextShower.Instance.ShowText(securedRoomCount, securedRoomCountText, () => {
                        StartCoroutine(WaitCoroutine(3f, () => {
                            animator.SetTrigger("hide");
                        }));
                    });
                });
            });
        });
    }

    IEnumerator WaitCoroutine(float seconds, Action onComplete = null)
    {
        yield return new WaitForSecondsRealtime(seconds);
        onComplete?.Invoke();
    }
}
