using UnityEngine;

public class QuotaSystem : MonoBehaviour
{
    public static QuotaSystem Instance = null;

    int required;
    int collected;
    int daysLeft;
    float multiplier = 1f;
    Order order;

    [Header("Links")]
    [SerializeField] QuotaUIController uiController;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public bool HasOrder() => order != null;
    public bool HasUncompletedOrder() => order != null && GetCollected() < GetRequired();
    public bool HasCompletedOrder() => order != null && GetCollected() >= GetRequired();
    public Order GetOrder() => order;
    public int GetClientTypeID() => HasOrder() ? ClientTypeManager.Instance.GetID(order.GetClientType()) : -1;
    public int GetRequired() => required;
    public int GetCollected() => collected;
    public int GetDaysLeft() => daysLeft;
    public float GetMultiplier() => multiplier;
    public void SetRequired(int required) => this.required = required;
    public void SetCollected(int collected) => this.collected = collected;
    public void SetDaysLeft(int daysLeft) => this.daysLeft = daysLeft;
    public void SetMultiplier(float multiplier) => this.multiplier = multiplier;
    public void SetOrder(Order order) => this.order = order;
    public void UpdateUI()
    {
        if (uiController != null)
            uiController.UpdateUI();
    }
}
