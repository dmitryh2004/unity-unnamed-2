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
    TypewiterAudioPlayer tap;

    private void Start()
    {
        tap = GetComponent<TypewiterAudioPlayer>();
    }

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

        currentCoroutine = StartCoroutine(ShowTextCoroutine(title, titleText));
        yield return new WaitUntil(() => currentCoroutine == null);

        currentCoroutine = StartCoroutine(ShowTextCoroutine(estimatedCost, estimatedCostText));
        yield return new WaitUntil(() => currentCoroutine == null);

        currentCoroutine = StartCoroutine(ShowTextCoroutine(protectedRoomCount, protectedRoomCountText));
        yield return new WaitUntil(() => currentCoroutine == null);

        currentCoroutine = StartCoroutine(ShowTextCoroutine(securedRoomCount, securedRoomCountText));
        yield return new WaitUntil(() => currentCoroutine == null);

        yield return new WaitForSecondsRealtime(3f);

        animator.SetTrigger("hide");
    }

    private IEnumerator ShowTextCoroutine(TMP_Text textElement, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            textElement.text += text[i];
            if (i % 3 == 0)
            {
                tap.PlayTypewriterSound();
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
        currentCoroutine = null;
    }
}
