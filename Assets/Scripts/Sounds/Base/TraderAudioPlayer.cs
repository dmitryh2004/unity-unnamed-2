using System.Collections.Generic;
using UnityEngine;

public class TraderAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> quotaCompletedAudios;
    [SerializeField] List<WeightedAudioClip> openWindowAudios;
    [SerializeField] List<WeightedAudioClip> closeWindowAudios;
    [SerializeField] List<WeightedAudioClip> actionAudios;

    public float PlayQuotaCompletedAudio()
    {
        return PlayRandomAudio(quotaCompletedAudios);
    }
    public float PlayOpenWindowAudio()
    {
        return PlayRandomAudio(openWindowAudios);
    }
    public float PlayCloseWindowAudio()
    {
        return PlayRandomAudio(closeWindowAudios);
    }
    public float PlayActionAudio()
    {
        return PlayRandomAudio(actionAudios);
    }
}
