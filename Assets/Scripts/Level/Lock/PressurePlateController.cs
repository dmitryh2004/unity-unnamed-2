using System.Collections.Generic;
using UnityEngine;

public class PressurePlateController : MonoBehaviour
{
    bool pressed = false;
    protected List<LootableItem> items = new();

    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] List<DoorController> controlledDoors = new ();

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
        return (items.Count > 0);
    }

    protected virtual bool CheckPressConditions()
    {
        return HasItemsWithinTriggerArea();
    }

    void UpdatePlateState()
    {
        pressed = CheckPressConditions();

        animator.SetBool("pressed", pressed);

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
