using UnityEngine;

public class GlobalAdaptiveDifficultyManager : MonoBehaviour
{
    AD_Locations locationsData = new();
    public static GlobalAdaptiveDifficultyManager Instance = null;
    [SerializeField] AdaptiveDifficultyValues values;

    [Header("Constants")]
    [SerializeField] float forgettingCoeff = 3f;
    [SerializeField] float remainingCoeff = .5f;

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
            }
            else
            {
                locationData.alertness = Mathf.Clamp(locationData.alertness - 1, -1, 5);
                foreach (var roomWeight in locationData.weights)
                {
                    roomWeight.weight = GetRecalculatedWeight(roomWeight.weight, 0);
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

    float GetRecalculatedWeight(float weight, float activity)
    {
        if (activity > 0) return weight + activity;
        else
        {
            float newWeight = weight - forgettingCoeff;
            if (newWeight < 0) newWeight = 0;
            if (newWeight / weight < remainingCoeff) newWeight = weight * remainingCoeff;
            return newWeight;
        }
    }
}
