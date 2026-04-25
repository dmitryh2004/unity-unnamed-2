using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScanner : UpgradableItem
{
    public static PlayerScanner Instance = null;
    [SerializeField] ProgressBarUIController uiController;
    [SerializeField] Image scannerIcon;
    [SerializeField] float checkGuardianMaxDistance = 20f;
    [SerializeField] float checkGuardianUpdatePositionPeriod = .5f;
    [SerializeField] AnimationCurve checkGuardianBeepPeriod = new ();
    [SerializeField] AnimationCurve checkGuardianBeepVolume = new ();
    [SerializeField] List<GuardianController> guardians = new ();
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip beepClip;
    float currentCharge = 0f;
    float chargeRegenSpeed = 0f;
    float chargeRegenDelay = 2f;
    float chargeUseSpeed = 1f;
    float regenDelayTimer = 0f;
    float maxCharge;
    bool checkGuardianDistance = false;
    float minGuardianDistance = float.PositiveInfinity;
    float guardianBeepPeriod = float.PositiveInfinity, guardianBeepVolume = 0f;
    float guardianBeepTimer = 0f;
    bool inUse = false;

    public int GetLootPrecision()
    {
        float? uv = GetUpgradableValue(0);
        if (uv == null) return -1;
        return (int)uv;
    }

    public int GetProtectedRoomPrecision()
    {
        float? uv = GetUpgradableValue(1);
        if (uv == null) return -1;
        return (int)uv;
    }

    public int GetSecuredRoomPrecision()
    {
        float? uv = GetUpgradableValue(2);
        if (uv == null) return -1;
        return (int)uv;
    }

    public float GetMaxCharge() => maxCharge;

    public float GetCurrentCharge() => currentCharge;

    public float GetChargeRegenDelay() => chargeRegenDelay;

    public bool InUse() => inUse;

    public void SetInUse(bool inUse) => this.inUse = inUse;

    public bool IsActive() => InUse() && GetCurrentCharge() > 0f;

    protected override void OnSetLevel()
    {
        base.OnSetLevel();
        AchievementActionTracker.Instance?.OnEquipmentLevelChanged("scanner", level);
        maxCharge = GetUpgradableValue(3) ?? 0;
        chargeRegenSpeed = GetUpgradableValue(4) ?? 0;
        chargeUseSpeed = GetUpgradableValue(5) ?? 0;
        chargeRegenDelay = GetUpgradableValue(6) ?? 0;
        checkGuardianDistance = GetUpgradableValue(7) != null ? GetUpgradableValue(7) > 0f : false;

        uiController.gameObject.SetActive(maxCharge != 0);
    }

    public void AddGuardian(GuardianController guardian)
    {
        if (!guardians.Contains(guardian)) guardians.Add(guardian);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (uiController == null)
        {
            uiController = GameObject.FindGameObjectWithTag("ScannerUI").GetComponent<ProgressBarUIController>();
        }
        if (scannerIcon == null)
        {
            scannerIcon = GameObject.FindGameObjectWithTag("ScannerUIIcon").GetComponent<Image>();
        }
    }

    private void Update()
    {
        UpdateGuardianDistance();
        UpdateGuardianBeep();
        UpdateDelayTimer();
        UpdateCurrentCharge();
        uiController.UpdateUI(currentCharge, maxCharge);
    }

    void UpdateGuardianDistance()
    {
        if (checkGuardianDistance)
        {
            float newMinDistance = float.PositiveInfinity;
            foreach (GuardianController guardian in guardians)
            {
                newMinDistance = Mathf.Min(newMinDistance, Vector3.Distance(transform.position, guardian.transform.position));
            }
            minGuardianDistance = newMinDistance;

            if (minGuardianDistance >= checkGuardianMaxDistance) return;

            float ratio = Mathf.Clamp(newMinDistance / checkGuardianMaxDistance, 0f, 1f);

            guardianBeepPeriod = checkGuardianBeepPeriod.Evaluate(ratio);
            guardianBeepVolume = checkGuardianBeepVolume.Evaluate(ratio);
        }
    }

    void UpdateGuardianBeep()
    {
        if (LevelManager.Instance.IsGameOver) return;

        guardianBeepTimer += Time.deltaTime;
        if (guardianBeepTimer >= guardianBeepPeriod)
        {
            guardianBeepTimer = 0f;
            audioSource.PlayOneShot(beepClip, guardianBeepVolume);
            StartCoroutine(BeepIcon(.2f));
        }
    }

    IEnumerator BeepIcon(float duration)
    {
        scannerIcon.color = Color.yellow;
        yield return new WaitForSeconds(duration);
        scannerIcon.color = Color.white;
    }

    void UpdateDelayTimer()
    {
        if (!inUse)
        {
            regenDelayTimer -= Time.deltaTime;
            if (regenDelayTimer < 0f) regenDelayTimer = 0f;
        }
        else
        {
            regenDelayTimer = chargeRegenDelay;
        }
    }

    void UpdateCurrentCharge()
    {
        float diff = 0f;
        if (inUse) diff = -chargeUseSpeed * Time.deltaTime;
        else
        {
            if (regenDelayTimer <= 0f)
            {
                diff = chargeRegenSpeed * Time.deltaTime;
            }
        }
        currentCharge = Mathf.Clamp(currentCharge + diff, 0, maxCharge);
    }
}
