using UnityEngine;

public class PlayerFlashlight : UpgradableItem
{
    public static PlayerFlashlight Instance = null;
    [SerializeField] ProgressBarUIController uiController;
    [SerializeField] Light flashlight;
    [SerializeField] PlayerAudioPlayer audioPlayer;
    float currentCharge = 0f;
    float chargeUseSpeed = 1f;
    float maxCharge = 0f;
    float intensity = 3f;
    bool inUse = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitLevel();

        if (uiController == null)
        {
            uiController = GameObject.FindGameObjectWithTag("FlashlightUI").GetComponent<ProgressBarUIController>();
        }
    }

    public float GetMaxCharge() => maxCharge;

    public float GetCurrentCharge() => currentCharge;

    public bool InUse() => inUse;

    public void SetInUse(bool inUse)
    {
        this.inUse = inUse;
        flashlight.gameObject.SetActive(inUse);
        if (inUse)
            audioPlayer.PlayFlashlightOnAudio();
        else
            audioPlayer.PlayFlashlightOffAudio();
    }

    public bool IsActive() => InUse() && GetCurrentCharge() > 0f;

    protected override void OnSetLevel()
    {
        base.OnSetLevel();
        currentCharge = maxCharge = GetUpgradableValue(0) ?? 0;
        chargeUseSpeed = GetUpgradableValue(1) ?? 0;
        intensity = GetUpgradableValue(2) ?? 3;

        flashlight.intensity = intensity;
    }

    private void Update()
    {
        float diff = (inUse ? -chargeUseSpeed : 0) * Time.deltaTime;
        currentCharge = Mathf.Clamp(currentCharge + diff, 0, maxCharge);

        if (currentCharge <= 0f && InUse())
        {
            SetInUse(false);
            audioPlayer.PlayFlashlightOutOfBatteryAudio();
        }

        uiController.UpdateUI(currentCharge, maxCharge);
    }
}
