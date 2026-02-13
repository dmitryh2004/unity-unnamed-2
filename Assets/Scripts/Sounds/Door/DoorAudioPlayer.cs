using System.Collections.Generic;
using UnityEngine;

public class DoorAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> openAudios;
    [SerializeField] List<WeightedAudioClip> closeAudios;
    [SerializeField] List<WeightedAudioClip> openStartAudios, openEndAudios, closeStartAudios, closeEndAudios;

    public float PlayOpenAudio()
    {
        return PlayRandomAudio(openAudios);
    }
    public float PlayCloseAudio()
    {
        return PlayRandomAudio(closeAudios);
    }
    public float PlayOpenStartAudio()
    {
        return PlayRandomAudio(openStartAudios);
    }
    public float PlayCloseStartAudio()
    {
        return PlayRandomAudio(closeStartAudios);
    }
    public float PlayOpenEndAudio()
    {
        return PlayRandomAudio(openEndAudios);
    }
    public float PlayCloseEndAudio()
    {
        return PlayRandomAudio(closeEndAudios);
    }
}
