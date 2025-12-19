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
        int lootPrecision = PlayerScanner.Instance.GetLootPrecision();
        int protectedRoomsPrecision = PlayerScanner.Instance.GetProtectedRoomPrecision();
        int securedRoomsPrecision = PlayerScanner.Instance.GetSecuredRoomPrecision();
        StartCoroutine(MainCoroutine(1.5f, lootPrecision, protectedRoomsPrecision, securedRoomsPrecision));
    }

    private IEnumerator MainCoroutine(float startDelay, int lootPrecision, int prPrecision, int secPrecision)
    {
        System.Random random = new ();
        yield return new WaitForSecondsRealtime(startDelay);

        int totalCost = generator.GetGeneratedLootSum();

        titleText = "Краткая сводка";
        estimatedCostText = $"Примерная стоимость вещей:\n";

        if (lootPrecision != -1)
        {
            if (lootPrecision > 0)
            {
                int minCost = totalCost / lootPrecision * lootPrecision;
                int maxCost = (totalCost / lootPrecision + 1) * lootPrecision;
                estimatedCostText += $"{NumberFormatter.FormatNumberWithGrouping(minCost)} - {NumberFormatter.FormatNumberWithGrouping(maxCost)}";
            }
            else if (lootPrecision == 0)
            {
                estimatedCostText += $"{NumberFormatter.FormatNumberWithGrouping(totalCost)}";
            }
        }
        else
        {
            estimatedCostText += "[неизвестно]";
        }

        estimatedCostText += " UMU";

        int protectedRooms = generator.GetProtectedRoomsCount();
        protectedRoomCountText = $"Число защищенных комнат: ";

        if (prPrecision != -1)
        {
            if (prPrecision > 0)
            {
                int prShift = random.Next(-prPrecision, prPrecision + 1), prFalloffA, prFalloffB;
                if (prShift < 0)
                {
                    prFalloffA = protectedRooms + prShift;
                    if (prFalloffA < 0) prFalloffA = 0;
                    prFalloffB = prFalloffA + prPrecision;
                }
                else
                {
                    prFalloffB = protectedRooms + prShift;
                    prFalloffA = prFalloffB - prPrecision;
                    if (prFalloffA < 0) prFalloffA = 0;
                }
                protectedRoomCountText += $"{prFalloffA} - {prFalloffB}";
            }
            else if (prPrecision == 0)
            {
                protectedRoomCountText += $"{protectedRooms}";
            }
        }
        else
        {
            protectedRoomCountText += $"[N/A]";
        }
        
        int securedRooms = generator.GetSecuredRoomsCount();
        securedRoomCountText = $"Число комнат-тайников: ";

        if (secPrecision != -1)
        {
            if (secPrecision > 0)
            {
                int secShift = random.Next(-secPrecision, secPrecision + 1), secFalloffA, secFalloffB;
                if (secShift < 0)
                {
                    secFalloffA = securedRooms + secShift;
                    if (secFalloffA < 0) secFalloffA = 0;
                    secFalloffB = secFalloffA + secPrecision;
                }
                else
                {
                    secFalloffB = securedRooms + secShift;
                    secFalloffA = secFalloffB - secPrecision;
                    if (secFalloffA < 0) secFalloffA = 0;
                }
                securedRoomCountText += $"{secFalloffA} - {secFalloffB}";
            }
            else if (secPrecision == 0)
            {
                securedRoomCountText += $"{securedRooms}";
            }
        }
        else
        {
            securedRoomCountText += $"[N/A]";
        }

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
