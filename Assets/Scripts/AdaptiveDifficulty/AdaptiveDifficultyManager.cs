using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdaptiveDifficultyManager : MonoBehaviour
{
    [Range(-1, 5)]
    [SerializeField] int forgettingDegree = 0;

    [Range(-1, 5)]
    [SerializeField] int alertnessDegree = 0;

    [SerializeField] AdaptiveDifficultyValues values;
    [SerializeField] AlertnessUIController uiController;
    public static AdaptiveDifficultyManager Instance = null;

    [SerializeField] bool useRoomWeights = false;
    [SerializeField] List<AD_RoomWeight> roomWeights = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        uiController?.UpdateUI(AlertnessDegree);
    }

    public AdaptiveDifficultyValues Values => values;
    public int ForgettingDegree => forgettingDegree;
    public int AlertnessDegree => alertnessDegree;
    public void SetForgettingDegree(int forgettingDegree) => this.forgettingDegree = forgettingDegree;
    public void SetAlertnessDegree(int alertnessDegree)
    {
        this.alertnessDegree = alertnessDegree;
        uiController?.UpdateUI(AlertnessDegree);
    }
    public bool UseRoomWeights => useRoomWeights;
    public List<AD_RoomWeight> RoomWeights => roomWeights;
    public void SetRoomWeights(List<AD_RoomWeight> roomWeights) => this.roomWeights = roomWeights;
    public string LocationName => SceneManager.GetActiveScene().name;
}
