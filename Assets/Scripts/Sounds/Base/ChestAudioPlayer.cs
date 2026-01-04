using System.Collections.Generic;
using UnityEngine;

public class ChestAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> openChestAudios;
    [SerializeField] List<WeightedAudioClip> closeChestAudios;

    public float PlayOpenChestAudio()
    {
        return PlayRandomAudio(openChestAudios);
    }
    public float PlayCloseChestAudio()
    {
        return PlayRandomAudio(closeChestAudios);
    }
}
