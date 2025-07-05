using System.Collections.Generic;
using UnityEngine;

public class TypewriterAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> typewriterAudios;

    public float PlayTypewriterSound()
    {
        return PlayRandomAudio(typewriterAudios);
    }
}
