using System.Collections.Generic;
using UnityEngine;

public class JewerlyTableAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> useAudios;

    public float PlayUseAudio()
    {
        return PlayRandomAudio(useAudios);
    }
}
