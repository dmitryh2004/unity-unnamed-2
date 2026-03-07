using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedAudioClip
{
    public AudioClip clip;
    public int weight;
    public float volume = 1f;

    WeightedAudioClip()
    {
        this.weight = 1;
        this.volume = 1f;
    }
}
[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAudioSource();
    }

    protected void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /**
     * <summary>
     * Воспроизводит звук. Возвращает его длительность в секундах.
     * </summary>
     * **/
    public float PlayRandomAudio(List<WeightedAudioClip> audios, Action<int> onComplete = null)
    {
        if (audios.Count == 0) return 0f;
        int totalWeight = 0;
        foreach (WeightedAudioClip clip in audios)
        {
            totalWeight += clip.weight;
        }

        int value = UnityEngine.Random.Range(0, totalWeight) + 1;
        int index = 0;
        int temp = 0;

        for (index = 0; index < audios.Count; index++)
        {
            temp += audios[index].weight;
            if (value <= temp) break;
        }
        
        audioSource.PlayOneShot(audios[index].clip, audios[index].volume);
        onComplete?.Invoke(index);
        return audios[index].clip.length;
    }
}