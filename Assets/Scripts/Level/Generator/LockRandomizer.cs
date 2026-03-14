using System.Collections.Generic;
using UnityEngine;

public class LockRandomizer : MonoBehaviour
{
    System.Random random = new();
    [SerializeField] List<LockController> locks;
    [SerializeField] int minDifficulty, maxDifficulty;
    void Awake()
    {
        foreach (LockController _lock in locks)
        {
            int difficulty = random.Next(minDifficulty, maxDifficulty + 1);
            _lock.SetDifficulty(difficulty);
        }
    }
}
