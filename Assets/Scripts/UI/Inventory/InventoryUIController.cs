using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    Dictionary<int, int> items;
    [SerializeField] TMP_Text totalVolume;
    [SerializeField] ProgressBar volumePB;
    [SerializeField] TMP_Text estimateCost;

    public void UpdateInventory()
    {
        items = InventorySystem.Instance.GetItems();

        float totalCost = 0f;

        foreach(int i in items.Keys)
        {
            Debug.Log($"{i} ({InventorySystem.Instance.GetLootCategoryById(i).lootName}): стоимость 1 шт.={InventorySystem.Instance.GetLootCategoryById(i).cost}");
            totalCost += items[i] * InventorySystem.Instance.GetLootCategoryById(i).cost;
        }

        float currentVolume = InventorySystem.Instance.GetOccupiedVolume();
        float maxVolume = InventorySystem.Instance.maxVolume;

        float ratio = currentVolume / maxVolume * 100;
        string format = (ratio < 10f) ? "0.0" : ((ratio < 100f) ? "00.0" : "000");

        totalVolume.text = $"{currentVolume} / {maxVolume} м3 ({(currentVolume / maxVolume * 100).ToString(format)}%)";
        volumePB.SetProgress(currentVolume);
        estimateCost.text = $"ќценочна€ стоимость вещей: {totalCost} руб.";
    }
}
