using System.Collections.Generic;
using UnityEngine;

public class GuardianAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> attackAudios;
    [SerializeField] List<WeightedAudioClip> pineappleAttackAudios;
    [SerializeField] List<WeightedAudioClip> footstepAudios;
    [SerializeField] List<WeightedAudioClip> rotateAudios;

	public float PlayPineappleAttackAudio() {
		return PlayRandomAudio(pineappleAttackAudios, (i) => {
            AchievementActionTracker.Instance?.OnGuardianSoundPlayed(16);
        });
	}

    public float PlayAttackAudio()
    {
        return PlayRandomAudio(attackAudios, (i) => {
            AchievementActionTracker.Instance?.OnGuardianSoundPlayed(i);
        });
    }

    public float PlayFootstepAudio()
    {
        return PlayRandomAudio(footstepAudios);
    }

    public float PlayRotateAudio()
    {
        return PlayRandomAudio(rotateAudios);
    }
}
