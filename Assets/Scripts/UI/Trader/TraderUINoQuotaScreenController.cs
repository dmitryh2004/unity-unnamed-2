using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] GameObject chosenTaskInfo;
    [SerializeField] GameObject noChosenTask;
    [Space(10)]
    [SerializeField] TMP_Text commonInfo;
    [SerializeField] TMP_Text earlyCompletionBonuses;
    [SerializeField] TMP_Text clientType;
    [SerializeField] TMP_Text lootCostModifiersPositive;
    [SerializeField] TMP_Text lootCostModifiersNegative;

    int chosenTask = 0; // 1-3 - task 1-3, 0 - no task
    ScrollRect chosenTaskInfoScrollRect;

    private void Awake()
    {
        chosenTaskInfoScrollRect = chosenTaskInfo.GetComponent<ScrollRect>();
    }
    public override void OnShow()
    {
        base.OnShow();
        taskCard1.SetOrder(trader.GetOrder1());
        taskCard2.SetOrder(trader.GetOrder2());
        taskCard3.SetOrder(trader.GetOrder3());

        UpdateTaskInfo();
    }

    public void SelectTask(int num)
    {
        chosenTask = num;
        UpdateTaskInfo();
    }

    void UpdateTaskInfo()
    {
        chosenTaskInfo.SetActive(chosenTask != 0);
        noChosenTask.SetActive(chosenTask == 0);
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
            ClientType currentClientType = currentOrder.GetClientType();

            commonInfo.text = $"- Необходимо собрать: {NumberFormatter.FormatNumberWithGrouping(currentOrder.GetRequired())}" +
                $"\n- Количество вылетов: {currentOrder.GetClientType().days}";

            if (currentClientType.hasEarlyCompletionBonus)
            {
                earlyCompletionBonuses.text = "Бонусы за досрочное выполнение:";
                if (currentClientType.earlyCompletionBonusModifier2 > 0f)
                {
                    earlyCompletionBonuses.text += $"\n- на 2 вылета раньше: х{currentClientType.earlyCompletionBonusModifier2} от заказа";
                }
                if (currentClientType.earlyCompletionBonusModifier1 > 0f)
                {
                    earlyCompletionBonuses.text += $"\n- на 1 вылет раньше: х{currentClientType.earlyCompletionBonusModifier1} от заказа";
                }
            }
            else
            {
                earlyCompletionBonuses.text = $"Нет бонусов за досрочное выполнение";
            }

            clientType.text = $"Клиент: {currentClientType.clientType}";

            List<LootCostModifier> lcmPositive = new(), lcmNegative = new();
            foreach (LootCostModifier lcm in currentClientType.lootCostModifiers)
            {
                if (lcm.modifier > 1f)
                    lcmPositive.Add(lcm);
                else if (lcm.modifier < 1f)
                    lcmNegative.Add(lcm);
            }

            lootCostModifiersPositive.gameObject.SetActive(lcmPositive.Count > 0);
            lootCostModifiersNegative.gameObject.SetActive(lcmNegative.Count > 0);

            if (lcmPositive.Count > 0)
            {
                lootCostModifiersPositive.text = "Предметы, за которые клиент готов платить больше:";
                foreach (LootCostModifier lcm in lcmPositive)
                {
                    LootCategory lc = lootCategories.lootCategories.FirstOrDefault((x) => x.id == lcm.itemID);
                    string lootName = lc.lootName;
                    float modifier = lcm.modifier;

                    int modifiedCost = (int)(lc.cost * modifier);

                    lootCostModifiersPositive.text += $"\n- {lootName} (x{NumberFormatter.FormatNumberWithGrouping(modifier)})\n" +
                        $"  Цена за 1 шт.: {NumberFormatter.FormatNumberWithGrouping(modifiedCost)} руб.";
                }
            }

            if (lcmNegative.Count > 0)
            {
                lootCostModifiersNegative.text = "Предметы, за которые клиент будет платить меньше:";
                foreach (LootCostModifier lcm in lcmNegative)
                {
                    LootCategory lc = lootCategories.lootCategories.FirstOrDefault((x) => x.id == lcm.itemID);
                    string lootName = lc.lootName;
                    float modifier = lcm.modifier;

                    int modifiedCost = (int)(lc.cost * modifier);

                    lootCostModifiersNegative.text += $"\n- {lootName} (x{NumberFormatter.FormatNumberWithGrouping(modifier)})\n" +
                        $"  Цена за 1 шт.: {NumberFormatter.FormatNumberWithGrouping(modifiedCost)} руб.";
                }
            }
        }

        chosenTaskInfoScrollRect.verticalNormalizedPosition = 1f;
    }
}
