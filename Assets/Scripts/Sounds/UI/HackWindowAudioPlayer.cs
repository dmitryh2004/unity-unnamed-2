using System.Collections.Generic;
using UnityEngine;

public class HackWindowAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> startHackAudios;
    [SerializeField] List<WeightedAudioClip> hackSuccessfulAudios;
    [SerializeField] List<WeightedAudioClip> hackFailedAudios;
    [SerializeField] List<WeightedAudioClip> openNodeAudios;
    [SerializeField] List<WeightedAudioClip> centralCoreFoundAudios;
    [SerializeField] List<WeightedAudioClip> doDamageAudios;
    [SerializeField] List<WeightedAudioClip> takeDamageAudios;
    [SerializeField] List<WeightedAudioClip> notAllowedAudios;
}
