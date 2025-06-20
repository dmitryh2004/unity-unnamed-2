using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Loot
{
    [Header("Префаб предмета")]
    public GameObject prefab;
    [Header("Вес выпадения для предмета")]
    public int weight;

    [Space(10)]
    [Header("Количество предметов")]
    [Min(1)]
    public int minCount;
    [Min(1)]
    public int maxCount;
}

public class LootContainer : MonoBehaviour
{
    System.Random random = new();
    [SerializeField] List<Loot> possibleLoot = new();
    [Range(0, 100)][SerializeField] int lootChance = 0;
    [SerializeField] Transform lootPointsParent;
    List<Transform> lootPoints = new();
    [Tooltip("Max loot points used.\n-1 = no limitations.")][SerializeField] int maxLootPointsUsed = -1;

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

    public void SpawnLoot()
    {
        int lootPointsUsed = 0;
        foreach (Transform lootPoint in lootPoints)
        {
            if (lootPointsUsed == maxLootPointsUsed) break;
            int chance = random.Next(1, 101);
            if (chance < lootChance)
            {
                int sum = 0;
                for (int i = 0; i < possibleLoot.Count; i++)
                {
                    sum += possibleLoot[i].weight;
                }
                int choice = random.Next(0, sum);

                sum = 0;
                Loot lootEntry = null;
                for (int i = 0; i < possibleLoot.Count; i++)
                {
                    sum += possibleLoot[i].weight;
                    if (sum >= choice)
                    {
                        lootEntry = possibleLoot[random.Next(0, possibleLoot.Count)];
                        break;
                    }
                }
                
                int lootCount = random.Next(lootEntry.minCount, lootEntry.maxCount + 1);

                for (int i = 0; i < lootCount; i++)
                {
                    Instantiate(lootEntry.prefab, lootPoint.position, lootPoint.rotation, lootPoint);
                }

                lootPointsUsed++;
            }
        }
    }
}
