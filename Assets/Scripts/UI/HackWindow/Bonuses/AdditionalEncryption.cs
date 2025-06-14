using UnityEngine;

public class AdditionalEncryption : Bonus
{
    public override void Initialize()
    {
        bonusType = BonusTypes.Instance.AdditionalEncryption;
        useDuration = false;
        int difficulty = HackWindowController.Instance.GetDifficulty();
        duration = bonusType.value1ByDifficulty[difficulty - 1];
        durationText.text = $"{duration}";
    }

    public override void MakeStep()
    {
        
    }

    public override void Use()
    {
        active = true;
        animator.SetBool("blink", true);
    }

    public void BlockAttack()
    {
        if (active)
        {
            duration--;
            if (duration == 0)
            {
                Destroy(gameObject);
                return;
            }
            durationText.text = $"{duration}";
        }
    }
}
