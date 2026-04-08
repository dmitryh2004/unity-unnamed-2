using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressurePlateTooltip : MonoBehaviour
{
    [SerializeField] PressurePlateController pressurePlateController;
    [SerializeField] TMP_Text text;
    [SerializeField] Transform player;
    [SerializeField] float maxOpacityRange = 5f;
    [SerializeField] float minOpacityRange = 10f;

    private void Update()
    {
        //update text rotation
        transform.LookAt(player.position, Vector3.up);
        transform.Rotate(Vector3.up * 180f);

        //update text opacity via alpha channel
        Color color = text.color;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < maxOpacityRange) color.a = 1f;
        else if (distance > minOpacityRange) color.a = 0f;
        else
        {
            float diff = minOpacityRange - maxOpacityRange;
            color.a = (minOpacityRange - distance) / diff;
        }

        text.color = color;
    }

    string GetEndingForTooltip(int itemCount)
    {
        if (itemCount % 100 > 10 && itemCount % 100 < 21)
        {
            return "любых предметов";
        }
        else if (itemCount % 10 == 1)
        {
            return "любой предмет";
        }
        else if (itemCount % 10 < 5)
        {
            return "любых предмета";
        }
        else
        {
            return "любых предметов";
        }
    }

    public void UpdateText()
    {
        string newText = "Условия активации:";

        bool checkItemCount = pressurePlateController.CheckItemCount();
        bool checkLootCategory = pressurePlateController.CheckLootCategory();

        if (!checkItemCount && !checkLootCategory)
        {
            newText += "\nПоложить 1 любой предмет";
        }
        else
        {
            if (checkItemCount)
            {
                int itemCount = pressurePlateController.GetRequiredItemCount();
                newText += $"\nКоличество предметов на плите: {itemCount}";
            }
            if (checkLootCategory)
            {
                List<LootCategory> lootCategories = pressurePlateController.GetAcceptableLootCategories();
                int requiredAcceptedItemsCount = pressurePlateController.GetRequiredCountOfAcceptedItems();

                newText += $"\n{requiredAcceptedItemsCount} {GetEndingForTooltip(requiredAcceptedItemsCount)} из списка:";

                foreach (LootCategory lc in lootCategories)
                {
                    newText += $"\n- {lc.lootName}";
                }
            }
        }

        text.text = newText;
    }
}
