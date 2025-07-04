using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedAudioClip
{
    public AudioClip clip;
    public int weight;
}
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
    public float PlayRandomAudio(List<WeightedAudioClip> audios)
    {
        int totalWeight = 0;
        foreach (WeightedAudioClip clip in audios)
        {
            totalWeight += clip.weight;
        }

        int value = Random.Range(0, totalWeight) + 1;
        int index = 0;
        int temp = 0;

        for (index = 0; index < audios.Count; index++)
        {
            temp += audios[index].weight;
            if (value <= temp) break;
        }

        audioSource.PlayOneShot(audios[index].clip);
        return audios[index].clip.length;
    }
}