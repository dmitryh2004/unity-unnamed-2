using System.Collections.Generic;
using UnityEngine;

public class SaveModuleAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> openWindowAudios;
    [SerializeField] List<WeightedAudioClip> closeWindowAudios;

    public float PlayOpenWindowAudio()
    {
        return PlayRandomAudio(openWindowAudios);
    }
    public float PlayCloseWindowAudio()
    {
        return PlayRandomAudio(closeWindowAudios);
    }
}
