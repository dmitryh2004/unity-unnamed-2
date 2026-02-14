using UnityEngine;

public class AdaptiveDifficultyManager : MonoBehaviour
{
    [Range(-1, 5)]
    [SerializeField] int forgettingDegree = 0;

    [Range(-1, 5)]
    [SerializeField] int alertnessDegree = 0;

    [SerializeField] AdaptiveDifficultyValues values;
    [SerializeField] AlertnessUIController uiController;
    public static AdaptiveDifficultyManager Instance = null;

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
    public void SetAlertnessDegree(int alertnessDegree) => this.alertnessDegree = alertnessDegree;
}
