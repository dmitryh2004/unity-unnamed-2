using UnityEngine;

public class GlobalAdaptiveDifficultyManager : MonoBehaviour
{
    AD_Locations locationsData = new();
    public static GlobalAdaptiveDifficultyManager Instance = null;
    [SerializeField] AdaptiveDifficultyValues values;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public AdaptiveDifficultyValues Values => values;
    public AD_Locations LocationsData => locationsData;
    public void SetLocationData(AD_Locations newData) => this.locationsData = newData;

    public void UpdateData(string chosenLocation)
    {
        foreach (var locationData in locationsData.locations)
        {
            if (locationData.locationName == chosenLocation)
            {
                locationData.alertness = Mathf.Clamp(locationData.alertness + 1, -1, 5);
                locationData.forgetting = 0;
            }
            else
            {
                locationData.alertness = Mathf.Clamp(locationData.alertness - 1, -1, 5);
                locationData.forgetting = Mathf.Clamp(locationData.forgetting + 1, 0, 5);
                if (locationData.forgetting == 5)
                {
                    foreach(var roomWeight in locationData.weights)
                    {
                        roomWeight.weight = 0;
                    }
                }
            }
        }
    }

    public int GetAlertnessDegree(string location)
    {
        foreach (var locationData in locationsData.locations)
        {
            if (locationData.locationName == location)
            {
                return locationData.alertness;
            }
        }
        return -1;
    }
}
