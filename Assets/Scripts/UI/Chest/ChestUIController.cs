using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestUIController : MonoBehaviour
{
    Dictionary<int, int> items;
    [SerializeField] Sprite unknownSprite;
    [Header("Inventory")]
    [SerializeField] List<InventoryItem> inventoryItems = new();
    int activeItemID = -1;
    int offset = 0;
    [SerializeField] TMP_Text totalVolume;
    [SerializeField] ProgressBar volumePB;
    [SerializeField] TMP_Text estimateCost;
    [Space(10)]
    [Header("Chest")]
    [SerializeField] List<InventoryItem> chestItems = new();
    int activeChestItemID = -1;
    int chestOffset = 0;
    [SerializeField] TMP_Text chestEstimateCost;
}
