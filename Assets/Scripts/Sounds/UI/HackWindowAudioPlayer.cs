using System.Collections.Generic;
using UnityEngine;

public class HackWindowAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> startHackAudios;
    [SerializeField] List<WeightedAudioClip> hackSuccessfulAudios;
    [SerializeField] List<WeightedAudioClip> hackFailedAudios;
    [SerializeField] List<WeightedAudioClip> openNodeAudios;
    [SerializeField] List<WeightedAudioClip> pickupBonusAudios;
    [SerializeField] List<WeightedAudioClip> useBonusAudios;
    [SerializeField] List<WeightedAudioClip> shieldBlockDamageAudios;
    [SerializeField] List<WeightedAudioClip> doDamageAudios;
    [SerializeField] List<WeightedAudioClip> takeDamageAudios;
    [SerializeField] List<WeightedAudioClip> notAllowedAudios;

    public float PlayStartHackAudio()
    {
        return PlayRandomAudio(startHackAudios);
    }
    public float PlayHackSuccessfulAudio()
    {
        return PlayRandomAudio(hackSuccessfulAudios);
    }
    public float PlayHackFailedAudio()
    {
        return PlayRandomAudio(hackFailedAudios);
    }
    public float PlayOpenNodeAudio()
    {
        return PlayRandomAudio(openNodeAudios);
    }
    public float PlayPickupBonusAudio()
    {
        return PlayRandomAudio(pickupBonusAudios);
    }
    public float PlayUseBonusAudio()
    {
        return PlayRandomAudio(useBonusAudios);
    }
    public float PlayShieldBlockDamageAudio()
    {
        return PlayRandomAudio(shieldBlockDamageAudios);
    }
    public float PlayDoDamageAudio()
    {
        return PlayRandomAudio(doDamageAudios);
    }
    public float PlayTakeDamageAudio()
    {
        return PlayRandomAudio(takeDamageAudios);
    }
    public float PlayNotAllowedAudio()
    {
        return PlayRandomAudio(notAllowedAudios);
    }
}
