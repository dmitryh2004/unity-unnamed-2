using System.Collections.Generic;
using UnityEngine;

public class GuardianManager : MonoBehaviour
{
    public static GuardianManager Instance = null;
    [SerializeField] Transform player;
    [SerializeField] List<GuardianController> guardians = new();

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
    }

    public void CallGuardians()
    {
        foreach (var guardian in guardians)
        {
            guardian.CallGuardian(player, player.position);
        }
    }

    public void StopGuardians()
    {
        foreach (var guardian in guardians)
        {
            guardian.SetActive(false);
        }
    }
}
