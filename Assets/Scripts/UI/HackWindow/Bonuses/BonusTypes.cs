using UnityEngine;

public class BonusTypes : MonoBehaviour
{
    public static BonusTypes Instance;
    public BonusType Reinforcement, Divider, AdditionalEncryption, AdditionalAttack;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
