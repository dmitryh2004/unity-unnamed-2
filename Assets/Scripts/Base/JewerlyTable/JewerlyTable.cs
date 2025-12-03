using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class JewerlyTable : Interactable
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] JewerlyTableUIController jewerlyTableUIController;
    [SerializeField] Transform spawnPoint;
    [Range(1, 7)]
    [SerializeField] int level = 1;
    System.Random random = new();

    public override void Interact()
    {
        jewerlyTableUIController.ShowWindow();
    }

    public int GetLevel() => level;
    public void SetLevel(int level) => this.level = level;

    public void ExecuteCraft(JewerlyTableCraft craft)
    {
        int totalWeight = 0, rand = 0;

        foreach(JewerlyTableOutputVariant ov in craft.outputVariants)
        {
            totalWeight += (int) ov.weight.FirstOrDefault((x) => x.level == level).value;
        }

        rand = random.Next(0, totalWeight);
        int weight = 0;
        int choice = 0;

        for (int i = 0; i < craft.outputVariants.Count; i++)
        {
            weight += (int) craft.outputVariants[i].weight.FirstOrDefault((x) => x.level == level).value;
            if (weight > rand)
            {
                choice = i;
                break;
            }
        }

        for (int i = 0; i < craft.requiredItems.Count; i++)
        {
            InventorySystem.Instance.RemoveItem(craft.requiredItems[i], craft.requiredItemsCount[i]);
        }

        for (int i = 0; i < craft.outputVariants[choice].outputItems.Count; i++)
        {
            int spawnAmount = random.Next(craft.outputVariants[choice].outputItemsAmount[i].min, craft.outputVariants[choice].outputItemsAmount[i].max);
            for (int j = 0; j < spawnAmount; j++)
            {
                GameObject output = Instantiate(craft.outputVariants[choice].outputItems[i].lootPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                output.transform.localScale = Vector3.one;
            }
        }

        jewerlyTableUIController.UpdateCraft();
    }
}
