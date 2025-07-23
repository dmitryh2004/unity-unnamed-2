using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DefeatCinematicController : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] GameObject commonScreen;
    [Space(10)]
    [SerializeField] GameObject defeatScreen;
    [Space]
    [SerializeField] TMP_Text statusCheck;
    [SerializeField] TMP_Text statusCheckCompleted;
    [SerializeField] TMP_Text deadline;
    [SerializeField] TMP_Text receivedResponse;
    [SerializeField] TMP_Text selfDestroy;
    [Space(10)]
    [SerializeField] Animator spaceshipAnimator;
    [Space(10)]
    [SerializeField] GameObject uiDefeatScreen;
    [Space(10)]
    [Header("Timers")]
    [SerializeField] float changeScreenDelay = 2f;
    [SerializeField] float startTypeStatusCheckDelay = 2f;
    [SerializeField] float startTypeStatusCheckCompletedDelay = 4f;
    [SerializeField] float startTypeDeadlineDelay = 1f;
    [SerializeField] float startTypeReceivedResponseDelay = 4f;
    [SerializeField] float startTypeSelfDestroyDelay = 2f;
    [SerializeField] float startAlarmDelay = 2f;
    [SerializeField] float startRotateShipDelay = 8f;
    [SerializeField] float openDoorDelay = 8f;
    [SerializeField] float showDefeatScreenDelay = 5f;
    //test
    private void Start()
    {
        StartCinematic();
    }
    public void StartCinematic()
    {
        StartCoroutine(CinematicCoroutine());
    }
    IEnumerator CinematicCoroutine()
    {
        string statusCheckText = statusCheck.text;
        string statusCheckCompletedText = statusCheckCompleted.text;
        string deadlineText = deadline.text;
        string receivedResponseText = receivedResponse.text;
        string selfDestroyText = selfDestroy.text;
        
        statusCheck.text = "";
        statusCheckCompleted.text = "";
        deadline.text = "";
        receivedResponse.text = "";
        selfDestroy.text = "";

        yield return new WaitForSeconds(changeScreenDelay);

        commonScreen.SetActive(false);
        defeatScreen.SetActive(true);

        yield return new WaitForSeconds(startTypeStatusCheckDelay);

        StartCoroutine(ShowTextCoroutine(statusCheck, statusCheckText, () =>
        {
            StartCoroutine(ShowTextCoroutine(statusCheckCompleted, statusCheckCompletedText, () =>
            {
                StartCoroutine(ShowTextCoroutine(deadline, deadlineText, () =>
                {
                    StartCoroutine(ShowTextCoroutine(receivedResponse, receivedResponseText, () =>
                    {
                        StartCoroutine(ShowTextCoroutine(selfDestroy, selfDestroyText, () =>
                        {

                        }, startTypeSelfDestroyDelay));
                    }, startTypeReceivedResponseDelay));
                }, startTypeDeadlineDelay));
            }, startTypeStatusCheckCompletedDelay));
        }, startTypeStatusCheckDelay));
    }

    IEnumerator ShowTextCoroutine(TMP_Text element, string text, Action onComplete, float delay)
    {
        yield return new WaitForSeconds(delay);
        TypewriterTextShower.Instance.ShowText(element, text, onComplete);
    }
}
