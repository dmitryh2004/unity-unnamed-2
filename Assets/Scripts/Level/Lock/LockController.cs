using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockController : Interactable
{
    [SerializeField] int startDifficulty;
    int difficulty;
    bool active = true;
    [SerializeField] GameObject screen;
    MeshRenderer screenRenderer;
    Rigidbody rb;

    [Space(10)]
    [Header("UI")]
    [SerializeField] TMP_Text difficultyText;
    [SerializeField] Image alarmSign;
    [SerializeField] TMP_Text alarmMinDifficultyText;

    [Space(10)]
    [Header("Links")]
    [SerializeField] List<LockController> linkedLocks = new();
    [SerializeField] List<Lockable> lockables = new();

    [Space(10)]
    [Header("Alarm Raiser")]
    [SerializeField] bool raiseAlarmOnFail = false;
    [Tooltip("При достижении этого уровня сложности поднимется тревога")][SerializeField] int raiseAlarmMinDifficulty = 11;
    int raiseAlarmDifficulty = 11;

    private void Awake()
    {
        difficulty = startDifficulty;
        rb = GetComponent<Rigidbody>();
        screenRenderer = screen.GetComponent<MeshRenderer>();
        screenRenderer.material = new Material(screenRenderer.material);
        raiseAlarmDifficulty = raiseAlarmMinDifficulty;
    }
    private void Start()
    {
        UpdateDifficultyScreen();
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    public int GetAlarmDifficulty()
    {
        return raiseAlarmDifficulty;
    }

    public bool RaiseAlarmOnFail => raiseAlarmOnFail;
    public void SetRaiseAlarmOnFail(bool raise)
    {
        raiseAlarmOnFail = raise;
    }
    
    public override void Interact()
    {
        if (IsActive() && IsHackable())
        {
            HackWindowController.Instance.OpenHackWindow(this);
            GetComponentInParent<RoomEventManager>()?.HackAttemptEvent();
        }
    }

    void CheckForAlarm()
    {
        if (AlarmController.Instance == null) return;
        if (raiseAlarmOnFail)
        {
            if (difficulty >= raiseAlarmDifficulty)
            {
                if (AlarmController.Instance.GetAlarmState() == false)
                {
                    Debug.Log($"Alarm raised (diff: {difficulty}, alarm diff: {raiseAlarmDifficulty}, raiser coords: {transform.position}");
                    AlarmController.Instance.StartAlarm();
                }
                GuardianManager.Instance.CallGuardians();
                GetComponentInParent<RoomEventManager>()?.AlarmRaisedEvent();
            }
        }
    }

    public void SetAlarmDifficulty(int diff, bool updateLinked = true)
    {
        raiseAlarmDifficulty = diff;
        raiseAlarmOnFail = true;
        CheckForAlarm();
        StartCoroutine(ChangeDifficultyOnScreenCoroutine(diff));
        if (updateLinked)
        {
            foreach (LockController linked in linkedLocks)
            {
                if (linked.IsActive()) linked.SetAlarmDifficulty(diff, updateLinked: false);
            }
        }
    }

    public void SetDifficulty(int diff, bool updateLinked = true)
    {
        difficulty = diff;
        CheckForAlarm();
        StartCoroutine(ChangeDifficultyOnScreenCoroutine(diff));
        if (updateLinked)
        {
            foreach (LockController linked in linkedLocks)
            {
                if (linked.IsActive()) linked.SetDifficulty(diff, updateLinked: false);
            }
        }
    }

    public void IncreaseDifficulty(int diff, bool updateLinked = true)
    {
        difficulty += diff;
        CheckForAlarm();
        StartCoroutine(ChangeDifficultyOnScreenCoroutine(diff));
        if (updateLinked)
        {
            StatisticCollector.Instance.FailedHacks++;
            if (difficulty >= 11)
            {
                StatisticCollector.Instance.LockedLocks++;
            }
            foreach (LockController linked in linkedLocks)
            {
                if (linked.IsActive()) linked.IncreaseDifficulty(diff, updateLinked: false);
            }
        }
    }

    IEnumerator ChangeDifficultyOnScreenCoroutine(int diff)
    {
        screen.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        UpdateDifficultyScreen();
    }

    public bool IsActive()
    {
        return active;
    }

    public bool IsHackable()
    {
        return difficulty < 11;
    }

    public void UpdateDifficultyScreen()
    {
        screen.SetActive(active);
        if (!active) return;

        if (IsHackable()) 
        {
            Color difficultyColor = new Color(Mathf.Clamp01(-0.25f + difficulty * 0.25f), Mathf.Clamp01(2.5f - difficulty * 0.25f), 0f);

            difficultyText.text = $"C: {difficulty}";
            difficultyText.color = difficultyColor;

            alarmSign.gameObject.SetActive(raiseAlarmOnFail);
            alarmSign.color = difficultyColor;

            int ramd = raiseAlarmDifficulty - 1;
            if (ramd < difficulty) ramd = difficulty;

            alarmMinDifficultyText.gameObject.SetActive(raiseAlarmOnFail);
            alarmMinDifficultyText.text = $"{ramd}";
            alarmMinDifficultyText.color = difficultyColor;
        }
        else
        {
            difficultyText.text = $"Locked";
            difficultyText.color = new Color(1f, 0f, 0f);

            alarmMinDifficultyText.gameObject.SetActive(false);

            alarmSign.gameObject.SetActive(false);
            
            screenRenderer.material.color = new Color(.5f, 0f, 0f);
            screenRenderer.material.SetColor("_EmissionColor", new Color(.25f, 0f, 0f));
        }
    }

    public void DisableLock(bool updateLinked = true, bool updateScreen = true)
    {
        active = false;
        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
        if (updateScreen)
            UpdateDifficultyScreen();
        if (updateLinked)
        {
            foreach (LockController linked in linkedLocks)
            {
                if (linked.IsActive()) linked.DisableLock(updateLinked: false, updateScreen: updateScreen);
            }
        }
        foreach (Lockable l in lockables)
        {
            l.UpdateLocked();
        }
    }

    public void RemoveLock(bool updateLinked = true)
    {
        DisableLock(updateLinked: updateLinked, updateScreen: false);

        if (updateLinked)
        {
            foreach (LockController linked in linkedLocks)
            {
                Destroy(linked.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
