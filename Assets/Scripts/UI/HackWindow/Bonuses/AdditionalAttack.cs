using UnityEngine;

public class AdditionalAttack : Bonus
{
    public override void Initialize()
    {
        bonusType = BonusTypes.Instance.AdditionalAttack;
        useTarget = true;
        int difficulty = HackWindowController.Instance.GetDifficulty();
        value1 = bonusType.value1ByDifficulty[difficulty - 1];
        duration = bonusType.value2ByDifficulty[difficulty - 1];
        durationText.text = $"{duration}";
    }

    public override void MakeStep()
    {
        if (active)
        {
            target.TakeDamage(value1);
            target.UpdateIcon();

            duration--;
            if (duration == 0)
            {
                Destroy(gameObject);
                return;
            }
            durationText.text = $"{duration}";
        }
    }

    public override void Use()
    {
        active = true;
        animator.SetBool("blink", true);
    }
}
