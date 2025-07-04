using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Loot
{
    [Header("Префаб предмета")]
    public GameObject prefab;
    [Header("Вес лута в группе")]
    public int weight;

    [Space(10)]
    [Header("Количество предметов")]
    [Min(1)]
    public int minCount;
    [Min(1)]
    public int maxCount;
}

[Serializable]
public class LootGroup
{
    [Header("Список лута в группе")]
    public List<Loot> possibleLoot;
    [Header("Вес группы лута")]
    public int weight;
}

public class LootContainer : MonoBehaviour
{
    System.Random random = new();
    [SerializeField] List<LootGroup> possibleLoot = new();
    [Range(0, 100)][SerializeField] int lootChance = 0;
    [SerializeField] Transform lootPointsParent;
    List<Transform> lootPoints = new();
    [Tooltip("Max loot points used.\n-1 = no limitations.")][SerializeField] int maxLootPointsUsed = -1;
    [SerializeField] bool hideIfNoLoot = true;

    public bool HideIfNoLoot() => hideIfNoLoot; 

    private void Awake()
    {
        int lootPointsCount = lootPointsParent.childCount;

        for (int i = 0; i < lootPointsCount; i++)
        {
            lootPoints.Add(lootPointsParent.GetChild(i));
        }

        if (maxLootPointsUsed == -1) maxLootPointsUsed = lootPointsCount;
    }

    public void HideLootContainer()
    {
        gameObject.SetActive(false);
    }

    public int SpawnLoot()
    {
        int lootSum = 0;
        int lootPointsUsed = 0;
        foreach (Transform lootPoint in lootPoints)
        {
            if (lootPointsUsed == maxLootPointsUsed) break;
            int chance = random.Next(1, 101);
            if (chance <= lootChance)
            {
                // выбираем группу лута
                int sum = 0;
                for (int i = 0; i < possibleLoot.Count; i++)
                {
                    sum += possibleLoot[i].weight;
                }
                int choice = random.Next(0, sum + 1);

                sum = 0;
                LootGroup lootGroup = null;
                for (int i = 0; i < possibleLoot.Count; i++)
                {
                    sum += possibleLoot[i].weight;
                    if (sum >= choice)
                    {
                        lootGroup = possibleLoot[i];
                        break;
                    }
                }

                // выбираем лут из группы
                sum = 0;
                for (int i = 0; i < lootGroup.possibleLoot.Count; i++)
                {
                    sum += lootGroup.possibleLoot[i].weight;
                }

                choice = random.Next(0, sum + 1);

                sum = 0;
                Loot lootEntry = null;
                for (int i = 0; i < lootGroup.possibleLoot.Count; i++)
                {
                    sum += lootGroup.possibleLoot[i].weight;
                    if (sum >= choice)
                    {
                        lootEntry = lootGroup.possibleLoot[i];
                        break;
                    }
                }
                
                int lootCount = random.Next(lootEntry.minCount, lootEntry.maxCount + 1);
                int lootPrice = lootEntry.prefab.GetComponent<LootableItem>().GetLootCategory().cost;
                lootSum += lootPrice * lootCount;

                for (int i = 0; i < lootCount; i++)
                {
                    Instantiate(lootEntry.prefab, lootPoint.position, lootPoint.rotation, lootPoint);
                }

                lootPointsUsed++;
            }
        }

        return lootSum;
    }
}
