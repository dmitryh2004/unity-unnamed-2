using System.Collections;
using UnityEngine;

public class DroneAudioController : MonoBehaviour
{
    [SerializeField] float activationChangeVolumeDuration = 3f;
    [SerializeField] float deactivationChangeVolumeDuration = 3f;
    [SerializeField] float activatedVolume = 1f, deactivatedVolume = 0f;
    float currentVolume;
    bool activated = false;
    bool changingVolume = false;
    [SerializeField] AudioSource source;
    float activationChangeVolumeRatio, deactivationChangeVolumeRatio;

    private void Start()
    {
        currentVolume = source.volume;
        if (activatedVolume < deactivatedVolume) activatedVolume = deactivatedVolume;
        activationChangeVolumeRatio = (activatedVolume - deactivatedVolume) / activationChangeVolumeDuration;
        deactivationChangeVolumeRatio = (deactivatedVolume - activatedVolume) / deactivationChangeVolumeDuration;
    }

    private void Update()
    {
        if (changingVolume)
        {
            if (activated)
            {
                if (currentVolume > activatedVolume)
                {
                    changingVolume = false;
                    currentVolume = activatedVolume;
                }
                else
                {
                    currentVolume += activationChangeVolumeRatio * Time.deltaTime;
                }
            }
            else
            {
                if (currentVolume < deactivatedVolume)
                {
                    changingVolume = false;
                    currentVolume = deactivatedVolume;
                }
                else
                {
                    currentVolume += deactivationChangeVolumeRatio * Time.deltaTime;
                }
            }
            source.volume = currentVolume;
        }
    }

    public void Activate()
    {
        changingVolume = true;
        activated = true;
    }

    public void Deactivate()
    {
        changingVolume = true;
        activated = false;
    }
}
