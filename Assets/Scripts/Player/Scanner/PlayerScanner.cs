using UnityEngine;

public class PlayerScanner : UpgradableItem
{
    public static PlayerScanner Instance = null;
    [SerializeField] ProgressBarUIController uiController;
    float currentCharge = 0f;
    float chargeRegenSpeed = 0f;
    float chargeUseSpeed = 1f;
    float maxCharge;
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

    public bool InUse() => inUse;

    public void SetInUse(bool inUse) => this.inUse = inUse;

    public bool IsActive() => InUse() && GetCurrentCharge() > 0f;

    protected override void OnSetLevel()
    {
        base.OnSetLevel();
        maxCharge = GetUpgradableValue(3) ?? 0;
        chargeRegenSpeed = GetUpgradableValue(4) ?? 0;
        chargeUseSpeed = GetUpgradableValue(5) ?? 0;

        uiController.gameObject.SetActive(maxCharge != 0);
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
    }

    private void Update()
    {
        float diff = (inUse ? -chargeUseSpeed : chargeRegenSpeed) * Time.deltaTime;
        currentCharge = Mathf.Clamp(currentCharge + diff, 0, maxCharge);

        uiController.UpdateUI(currentCharge, maxCharge);
    }
}
