using UnityEngine;

public class QuotaSystem : MonoBehaviour
{
    public static QuotaSystem Instance = null;

    int required;
    int collected;
    int daysLeft;
    float multiplier;
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
    public Order GetOrder() => order;
    public int GetRequired() => required;
    public int GetCollected() => collected;
    public int GetDaysLeft() => daysLeft;
    public float GetMultiplier() => multiplier;
    public void UpdateUI()
    {
        uiController.UpdateUI();
    }
}
