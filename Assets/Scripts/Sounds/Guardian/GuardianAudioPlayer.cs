using System.Collections.Generic;
using UnityEngine;

public class GuardianAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> attackAudios;

    public float PlayAttackAudio()
    {
        return PlayRandomAudio(attackAudios);
    }
}
