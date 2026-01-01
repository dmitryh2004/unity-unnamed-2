using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskInfoElement : MonoBehaviour
{
    [SerializeField] bool taskAccepted = false;
    [SerializeField] LootCategoryManager lootCategories;
    [Space(10)]
    [SerializeField] ScrollRect taskInfoScrollRect;
    [Space(10)]
    [SerializeField] TMP_Text commonInfo;
    [SerializeField] TMP_Text earlyCompletionBonuses;
    [SerializeField] TMP_Text clientType;
    [SerializeField] TMP_Text lootCostModifiersPositive;
    [SerializeField] TMP_Text lootCostModifiersNegative;

    public void UpdateTaskInfo(Order order)
    {
        ClientType currentClientType = order.GetClientType();

        commonInfo.text = $"- Необходимо собрать: {NumberFormatter.FormatNumberWithGrouping(order.GetRequired())}";
        if (taskAccepted)
        {
            commonInfo.text += $"\n- Собрано: {NumberFormatter.FormatNumberWithGrouping(QuotaSystem.Instance.GetCollected())}";
        }
        commonInfo.text += $"\n- Количество вылетов: {order.GetClientType().days}";

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

        List<LootCostModifier> lcmPositive = new (), lcmNegative = new ();
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

                lootCostModifiersPositive.text += $"\n- {lootName} <color=#00ff00>(x{NumberFormatter.FormatNumberWithGrouping(modifier)})</color>\n" +
                    $"  Цена за 1 шт.: {NumberFormatter.FormatNumberWithGrouping(modifiedCost)} UMU";
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

                lootCostModifiersNegative.text += $"\n- {lootName} <color=#ff0000>(x{NumberFormatter.FormatNumberWithGrouping(modifier)})</color>\n" +
                    $"  Цена за 1 шт.: {NumberFormatter.FormatNumberWithGrouping(modifiedCost)} UMU";
            }
        }

        taskInfoScrollRect.verticalNormalizedPosition = 1f;
    }
}
