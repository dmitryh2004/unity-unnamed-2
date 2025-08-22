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

    [Header("Call Guardians Options")]
    [SerializeField] bool checkDistance = false;
    [SerializeField] float maxAggroDistance = 100f;

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
            bool call = !checkDistance || (checkDistance && (Vector3.Distance(player.position, guardian.transform.position) <= maxAggroDistance));
            if (!call) continue;

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
