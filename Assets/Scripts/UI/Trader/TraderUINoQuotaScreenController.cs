using UnityEngine;
using UnityEngine.UI;

public class TraderUINoQuotaScreenController : TraderUIBaseScreenController
{
    [Header("Links")]
    [SerializeField] TraderUIWindowController windowController;
    [Space(10)]
    [SerializeField] TraderObject trader;
    [SerializeField] LootCategoryManager lootCategories;
    [Header("Task cards")]
    [SerializeField] TaskCard taskCard1;
    [SerializeField] TaskCard taskCard2;
    [SerializeField] TaskCard taskCard3;
    [Header("Selected task")]
    [SerializeField] TaskInfoElement chosenTaskInfoElement;
    [SerializeField] GameObject noChosenTask;
    [Space(10)]
    [SerializeField] Button acceptTaskBtn;

    int chosenTask = 0; // 1-3 - task 1-3, 0 - no task
    ScrollRect chosenTaskInfoScrollRect;

    private void Awake()
    {
        chosenTaskInfoScrollRect = chosenTaskInfoElement.GetComponent<ScrollRect>();
    }
    public override void OnShow()
    {
        base.OnShow();

        taskCard1.SetOrder(trader.GetOrder1());
        taskCard2.SetOrder(trader.GetOrder2());
        taskCard3.SetOrder(trader.GetOrder3());

        SelectTask(0);
    }

    public void SelectTask(int num)
    {
        chosenTask = num;
        UpdateTaskCards();
        UpdateTaskInfo();
    }

    void UpdateTaskCards()
    {
        taskCard1.SetSelected(chosenTask == 1);
        taskCard2.SetSelected(chosenTask == 2);
        taskCard3.SetSelected(chosenTask == 3);
    }

    void UpdateTaskInfo()
    {
        chosenTaskInfoElement.gameObject.SetActive(chosenTask != 0);
        noChosenTask.SetActive(chosenTask == 0);
        acceptTaskBtn.gameObject.SetActive(chosenTask != 0);
        if (chosenTask != 0)
        {
            Order currentOrder = null;
            switch (chosenTask)
            {
                case 1:
                    currentOrder = trader.GetOrder1();
                    break;
                case 2:
                    currentOrder = trader.GetOrder2();
                    break;
                case 3:
                    currentOrder = trader.GetOrder3();
                    break;
            }
            chosenTaskInfoElement.UpdateTaskInfo(currentOrder);
        }
    }
    public void AcceptTask()
    {
        if (chosenTask < 1 || chosenTask > 3) return;
        Order selectedOrder = null;
        switch (chosenTask)
        {
            case 1:
                selectedOrder = trader.GetOrder1();
                break;
            case 2:
                selectedOrder = trader.GetOrder2();
                break;
            case 3:
                selectedOrder = trader.GetOrder3();
                break;
        }

        QuotaSystem.Instance.SetOrder(selectedOrder);
        QuotaSystem.Instance.SetRequired(selectedOrder.GetRequired());
        QuotaSystem.Instance.SetCollected(0);
        QuotaSystem.Instance.SetMultiplier(selectedOrder.GetMultiplier());
        QuotaSystem.Instance.SetDaysLeft(selectedOrder.GetClientType().days);
        QuotaSystem.Instance.UpdateUI();

        windowController.SetScreen(1);
    }
}
