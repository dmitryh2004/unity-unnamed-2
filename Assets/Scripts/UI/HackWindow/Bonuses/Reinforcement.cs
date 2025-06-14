using UnityEngine;

public class Reinforcement : Bonus
{
    public override void Initialize()
    {
        bonusType = BonusTypes.Instance.Reinforcement;
        int difficulty = HackWindowController.Instance.GetDifficulty();
        duration = bonusType.value1ByDifficulty[difficulty - 1];
        int bonusValue2 = bonusType.value2ByDifficulty[difficulty - 1];
        int bonusValue3 = bonusType.value3ByDifficulty[difficulty - 1];
        value1 = random.Next(bonusValue2, bonusValue3 + 1);
    }

    public override void MakeStep()
    {
        if (active)
        {
            VirusController.Instance.Heal(value1);
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
