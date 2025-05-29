using UnityEngine;

public class LootableItem : Interactable
{
    [SerializeField] LootCategory lootCategory;

    public LootCategory GetLootCategory()
    {
        return lootCategory;
    }

    public override void Interact()
    {
        if (InventorySystem.Instance.CanAddItem(lootCategory))
        {
            InventorySystem.Instance.AddItem(lootCategory);
            Destroy(gameObject);
        }
    }
}
