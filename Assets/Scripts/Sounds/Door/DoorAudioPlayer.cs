using System.Collections.Generic;
using UnityEngine;

public class DoorAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> openAudios;
    [SerializeField] List<WeightedAudioClip> closeAudios;
    [SerializeField] List<WeightedAudioClip> creakAudios;

    public float PlayOpenAudio()
    {
        return PlayRandomAudio(openAudios);
    }
    public float PlayCloseAudio()
    {
        return PlayRandomAudio(closeAudios);
    }
    public float PlayCreakAudio()
    {
        return PlayRandomAudio(creakAudios);
    }
}
