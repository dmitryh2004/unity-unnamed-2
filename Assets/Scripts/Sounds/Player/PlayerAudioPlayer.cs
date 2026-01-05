using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> footstepAudios;
    [SerializeField] List<WeightedAudioClip> balanceChangedAudios;
    [SerializeField] List<WeightedAudioClip> flashlightOnAudios;
    [SerializeField] List<WeightedAudioClip> flashlightOffAudios;

    public float PlayFootstepAudio()
    {
        return PlayRandomAudio(footstepAudios);
    }

    public float PlayBalanceChangedAudio()
    {
        return PlayRandomAudio(balanceChangedAudios);
    }

    public float PlayFlashlightOnAudio()
    {
        return PlayRandomAudio(flashlightOnAudios);
    }
    public float PlayFlashlightOffAudio()
    {
        return PlayRandomAudio(flashlightOffAudios);
    }
}
