using UnityEngine;

public class BonusController : MonoBehaviour
{
    public static BonusController Instance = null;
    [SerializeField] GameObject reinforcementPrefab, dividerPrefab, additionalEncryptionPrefab, additionalAttackPrefab;
    [SerializeField] BonusSlot bonusSlot1, bonusSlot2, bonusSlot3;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ClearBonuses()
    {
        for (int i = 1; i < 4; i++)
        {
            Bonus bonus = GetBonus(i);
            if (bonus != null)
                Destroy(bonus.gameObject);
        }
        bonusSlot1.SetBonus(null);
        bonusSlot2.SetBonus(null);
        bonusSlot3.SetBonus(null);
    }

    BonusSlot FindEmptyBonusSlot()
    {
        if (bonusSlot1.Empty()) return bonusSlot1;
        if (bonusSlot2.Empty()) return bonusSlot2;
        if (bonusSlot3.Empty()) return bonusSlot3;
        return null;
    }

    public Bonus GetBonus(int slotNumber)
    {
        if (slotNumber == 1) return bonusSlot1.GetBonus();
        if (slotNumber == 2) return bonusSlot2.GetBonus();
        if (slotNumber == 3) return bonusSlot3.GetBonus();
        return null;
    }

    public bool AddBonus(BonusType bonusType)
    {
        BonusSlot slot = FindEmptyBonusSlot();
        if (slot == null) return false;

        GameObject prefab = null;
        switch (bonusType.id)
        {
            case 1:
                prefab = reinforcementPrefab;
                break;
            case 2:
                prefab = dividerPrefab;
                break;
            case 3:
                prefab = additionalEncryptionPrefab;
                break;
            case 4:
                prefab = additionalAttackPrefab;
                break;
        }

        GameObject newBonusGO = GameObject.Instantiate(prefab, slot.transform);
        Bonus newBonus = newBonusGO.GetComponent<Bonus>();
        newBonus.Initialize();

        slot.SetBonus(newBonus);
        return true;
    }

    public void MakeStepPost()
    {
        if (!bonusSlot1.Empty()) bonusSlot1.GetBonus().MakeStep();
        if (!bonusSlot2.Empty()) bonusSlot2.GetBonus().MakeStep();
        if (!bonusSlot3.Empty()) bonusSlot3.GetBonus().MakeStep();
    }
}
