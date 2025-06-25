using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [SerializeField] List<LootContainer> containers;

    public int SpawnLoot()
    {
        int sum = 0;
        foreach (LootContainer lc in containers)
        {
            sum += lc.SpawnLoot();
        }
        return sum;
    }
}
