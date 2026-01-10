using System.Collections.Generic;
using UnityEngine;

public class PressurePlateController : MonoBehaviour
{
    bool pressed = false;
    protected List<LootableItem> items = new();
    [Header("Pressure Plate Settings")]
    [SerializeField] bool checkItemCount = false;
    [SerializeField] int requiredCountOfItems = 1;

    [Space(10)]
    [SerializeField] bool checkLootCategory = false;
    [SerializeField] List<LootCategory> acceptableLootCategories = new ();
    [SerializeField] int requiredCountOfAcceptedItems = 1;

    [Space(10)]
    [SerializeField] float checkItemsTime = 1f;

    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] List<DoorController> controlledDoors = new ();
    [SerializeField] PressurePlateTooltip tooltip;
    [SerializeField] PressurePlateEmissionController emissionController;

    private void Start()
    {
        tooltip.UpdateText();
        InvokeRepeating(nameof(CheckItems), 0f, checkItemsTime);
    }

    void CheckItems()
    {
        List<LootableItem> newList = new ();

        foreach (var item in items)
        {
            if (item != null && item.gameObject != null)
            {
                newList.Add(item);
            }
        }

        items = newList;
        UpdatePlateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        LootableItem li = null;
        if (other.TryGetComponent<LootableItem>(out li))
        {
            items.Add(li);
            UpdatePlateState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LootableItem li = null;
        if (other.TryGetComponent<LootableItem>(out li))
        {
            if (items.Contains(li)) items.Remove(li);
            UpdatePlateState();
        }
    }

    protected bool HasItemsWithinTriggerArea()
    {
        return (GetItemsCount() > 0);
    }

    protected int GetItemsCount()
    {
        return items.Count;
    }

    public bool CheckItemCount() => checkItemCount;
    public int GetRequiredItemCount() => requiredCountOfItems;
    public bool CheckLootCategory() => checkLootCategory;
    public int GetRequiredCountOfAcceptedItems() => requiredCountOfAcceptedItems;
    public List<LootCategory> GetAcceptableLootCategories() => acceptableLootCategories;


    protected virtual bool CheckPressConditions()
    {
        bool hasItems = HasItemsWithinTriggerArea();
        bool result = hasItems;

        if (result && checkItemCount)
        {
            result = (GetItemsCount() > requiredCountOfItems);
        }

        if (result && checkLootCategory)
        {
            bool hasItemWithAcceptedLootCategory = false;
            int acceptedItemsCount = 0;

            foreach (LootableItem item in items)
            {
                if (acceptableLootCategories.Contains(item.GetLootCategory()))
                {
                    hasItemWithAcceptedLootCategory = true;
                    acceptedItemsCount++;
                }
            }

            result = hasItemWithAcceptedLootCategory && acceptedItemsCount >= requiredCountOfAcceptedItems;
        }

        return result;
    }

    void UpdatePlateState()
    {
        pressed = CheckPressConditions();

        animator.SetBool("pressed", pressed);
        emissionController.SetEmitting(pressed);

        UpdateDoors();
    }

    void UpdateDoors()
    {
        foreach (DoorController dc in controlledDoors)
        {
            dc.ChangeDoorState(pressed);
        }
    }
}
