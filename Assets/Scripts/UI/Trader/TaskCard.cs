using TMPro;
using UnityEngine;

public class TaskCard : MonoBehaviour
{
    [SerializeField] TMP_Text required, days, client;
    Order order;
    
    public void UpdateText()
    {
        required.text = $"Сумма: {NumberFormatter.FormatNumberWithGrouping(order.GetRequired())}";
        days.text = $"Вылетов: {order.GetClientType().days}";
        client.text = $"Клиент: {order.GetClientType().clientType}";
    }

    public void SetOrder(Order order)
    {
        this.order = order;
        UpdateText();
    }
}
