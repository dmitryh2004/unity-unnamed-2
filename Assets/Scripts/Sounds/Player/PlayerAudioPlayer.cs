using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> footstepAudios;

    public float PlayFootstepAudio()
    {
        return PlayRandomAudio(footstepAudios);
    }
}
