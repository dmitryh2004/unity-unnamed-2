using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    public static DroneManager Instance = null;
    [SerializeField] List<DroneController> drones = new ();
    [SerializeField] int requiredDronesCount = 1;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (AdaptiveDifficultyManager.Instance != null)
        {
            float requiredDronesRatio = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("RequiredDronesRatio", AdaptiveDifficultyManager.Instance.AlertnessDegree()) ?? .5f;
            requiredDronesCount = (int) Mathf.Ceil(requiredDronesRatio * drones.Count);
        }

        if (requiredDronesCount < drones.Count)
        {
            List<int> droneIndexes = RandomNumbers.GetUniqueRandomNumbers(requiredDronesCount, 0, drones.Count - 1);
            for (int i = 0; i < drones.Count; i++)
            {
                if (!droneIndexes.Contains(i))
                {
                    drones[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void EnableDrones()
    {
        foreach (DroneController dc in drones)
        {
            if (dc.gameObject.activeInHierarchy)
                dc.Launch();
        }
    }
}
