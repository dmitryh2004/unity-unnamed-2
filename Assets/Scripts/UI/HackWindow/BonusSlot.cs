using UnityEngine;

public class BonusSlot : MonoBehaviour
{
    Bonus bonus;
    
    public void SetBonus(Bonus bonus)
    {
        this.bonus = bonus;
    }

    public bool Empty() => bonus == null;

    public Bonus GetBonus() => bonus;
}
