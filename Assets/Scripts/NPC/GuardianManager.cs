using System.Collections.Generic;
using UnityEngine;

public class GuardianManager : MonoBehaviour
{
    public static GuardianManager Instance = null;
    [SerializeField] Transform player;
    [SerializeField] List<GuardianController> guardians = new();

    [Header("Guardians initial spawn")]
    [SerializeField] bool removeRedundantGuardians = true;
    [SerializeField] int minGuardians;
    [SerializeField] int maxGuardians;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;

        if (removeRedundantGuardians)
        {
            System.Random random = new();
            int count = random.Next(minGuardians, maxGuardians + 1);
            List<int> temp = RandomNumbers.GetUniqueRandomNumbers(count, 0, maxGuardians - 1);

            for (int i = 0; i < guardians.Count; i++)
            {
                if (!temp.Contains(i))
                {
                    guardians[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void CallGuardians()
    {
        foreach (var guardian in guardians)
        {
            if (guardian.gameObject.activeInHierarchy)
                guardian.CallGuardian(player, player.position);
        }
    }

    public void StopGuardians()
    {
        foreach (var guardian in guardians)
        {
            if (guardian.gameObject.activeInHierarchy)
                guardian.SetActive(false);
        }
    }
}
