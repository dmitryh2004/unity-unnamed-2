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
    [SerializeField] Rigidbody playerRb;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator spaceshipAnimator;
    [Space(10)]
    [SerializeField] GameObject uiDefeatScreen;
    [SerializeField] Animator uiDefeatScreenAnimator;
    [SerializeField] TMP_Text uiDefeatScreenTitle;
    [SerializeField] TMP_Text uiDefeatScreenText;
    [Space(10)]
    [SerializeField] ExitGame exitGame;
    [Header("Timers")]
    [SerializeField] float changeScreenDelay = 2f;
    [Space]
    [SerializeField] float startTypeStatusCheckDelay = 2f;
    [SerializeField] float startTypeStatusCheckCompletedDelay = 4f;
    [SerializeField] float startTypeDeadlineDelay = 1f;
    [SerializeField] float startTypeReceivedResponseDelay = 4f;
    [SerializeField] float startTypeSelfDestroyDelay = 2f;
    [Space]
    [SerializeField] float startAlarmDelay = 2f;
    [SerializeField] float startRotateShipDelay = 8f;
    [SerializeField] float openDoorDelay = 8f;
    [Space]
    [SerializeField] float showDefeatScreenDelay = 5f;
    [Space]
    [SerializeField] float startTypeDefeatScreenTitle = 2f;
    [SerializeField] float startTypeDefeatScreenText = 1f;
    [SerializeField] float exitSceneDelay = 10f;

    string defeatScreenTitleText, defeatScreenTextText;

    private void Awake()
    {
        defeatScreenTitleText = uiDefeatScreenTitle.text;
        defeatScreenTextText = uiDefeatScreenText.text;

        uiDefeatScreenTitle.text = "";
        uiDefeatScreenText.text = "";

        uiDefeatScreen.SetActive(false);
    }
    //test
    private void Start()
    {
        StartCinematic();
    }
    public void StartCinematic()
    {
        StartCoroutine(CinematicCoroutineP1());
    }
    IEnumerator CinematicCoroutineP1()
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
                            StartCoroutine(CinematicCoroutineP2());
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

    IEnumerator CinematicCoroutineP2()
    {
        yield return new WaitForSeconds(startAlarmDelay);

        AlarmController.Instance.StartAlarm();
        StartCoroutine(AnimatePlayerWalkingAway());

        yield return new WaitForSeconds(startRotateShipDelay);

        spaceshipAnimator.SetTrigger("Rotate");

        yield return new WaitForSeconds(openDoorDelay);

        spaceshipAnimator.SetTrigger("OpenDoor");

        yield return new WaitForSeconds(showDefeatScreenDelay);

        uiDefeatScreen.SetActive(true);
        uiDefeatScreenAnimator.SetTrigger("Show");

        yield return new WaitForSeconds(0.5f);

        

        StartCoroutine(ShowTextCoroutine(uiDefeatScreenTitle, defeatScreenTitleText, () => {
            StartCoroutine(ShowTextCoroutine(uiDefeatScreenText, defeatScreenTextText, () => {
                StartCoroutine(ExitScene(exitSceneDelay));
            }, startTypeDefeatScreenText));
        }, startTypeDefeatScreenTitle));
    }

    private IEnumerator AnimatePlayerWalkingAway()
    {
        playerRb.Sleep();
        playerAnimator.SetTrigger("WalkAway");
        yield return new WaitForSeconds(3f);
        playerAnimator.enabled = false;
        playerRb.WakeUp();
    }

    private IEnumerator ExitScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        uiDefeatScreenAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);

        exitGame.Exit();
    }
}
