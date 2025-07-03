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

    private void Start()
    {
        StartCoroutine(MainCoroutine(0.5f, 1000000));
    }

    private IEnumerator MainCoroutine(float startDelay, int precision)
    {
        yield return new WaitForSecondsRealtime(startDelay);

        int totalCost = generator.GetGeneratedLootSum();

        int minCost = totalCost / precision * precision;
        int maxCost = (totalCost / precision + 1) * precision;

        titleText = "Краткая сводка";
        estimatedCostText = $"Примерная стоимость вещей:\n{NumberFormatter.FormatNumberWithGrouping(minCost)} - {NumberFormatter.FormatNumberWithGrouping(maxCost)}";

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
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
        currentCoroutine = null;
    }
}
