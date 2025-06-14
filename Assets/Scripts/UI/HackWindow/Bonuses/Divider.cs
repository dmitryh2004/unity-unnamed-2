using UnityEngine;

public class Divider : Bonus
{
    public override void Initialize()
    {
        bonusType = BonusTypes.Instance.Divider;
        useTarget = true;
        useDuration = false;
        int difficulty = HackWindowController.Instance.GetDifficulty();
        value1 = bonusType.value1ByDifficulty[difficulty - 1];
    }

    public override void MakeStep()
    {
        
    }

    public override void Use()
    {
        int targetHP = target.GetCurrentHP();

        int damage = targetHP * value1 / 100;

        target.TakeDamage(damage);
        target.UpdateIcon();

        Destroy(gameObject);
    }
}
