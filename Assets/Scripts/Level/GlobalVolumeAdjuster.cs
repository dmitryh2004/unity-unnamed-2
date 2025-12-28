using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeAdjuster : MonoBehaviour
{
    [SerializeField] VolumeProfile volumeProfile;
    ColorAdjustments colorAdjustments;

    float savedPostExposure = 0f, savedContrast = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            savedPostExposure = PlayerPrefs.GetFloat("PostExposure", 0f);
            savedContrast = PlayerPrefs.GetFloat("Contrast", 0f);

            colorAdjustments.postExposure.value = savedPostExposure;
            colorAdjustments.contrast.value = savedContrast;
        }
    }
}
