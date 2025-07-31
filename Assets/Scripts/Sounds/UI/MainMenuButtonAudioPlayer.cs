using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonAudioPlayer : RandomAudioPlayer
{
    [SerializeField] List<WeightedAudioClip> buttonClickSounds, buttonClickIncorrectSounds;

    public float PlayClickSound()
    {
        return PlayRandomAudio(buttonClickSounds);
    }

    public float PlayClickIncorrectSound()
    {
        return PlayRandomAudio(buttonClickIncorrectSounds);
    }

    public void PlayClickSound_NoReturn()
    {
        PlayClickSound();
    }
}
