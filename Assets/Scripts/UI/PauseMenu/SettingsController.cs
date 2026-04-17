using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
	[SerializeField] Volume volume;
    [SerializeField] VolumeProfile volumeProfile;
    ColorAdjustments colorAdjustments;

    float postExposure = 0f, contrast = 0f;
    float savedPostExposure = 0f, savedContrast = 0f;
    [SerializeField] Slider postExposureSlider, contrastSlider;
    bool initialized = false;
	
	void Start() {
		Init();
	}

    void Init()
    {
		if (initialized) return;
		if (volume != null) {
			volume.profile = volumeProfile;
			
			if (volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments))
			{
				ApplySavedValues();

				postExposure = savedPostExposure;
				contrast = savedContrast;

				initialized = true;
			}

		}
    }

    void UpdateVolumeProfile(bool useSavedValues = false, bool checkInit = true)
    {
        if (checkInit && !initialized)
        {
            Init();
        }
        colorAdjustments.postExposure.value = useSavedValues ? savedPostExposure : postExposure;
        colorAdjustments.contrast.value = useSavedValues ? savedContrast : contrast;
    }

    public void OnPostExposureValueChanged(float newValue)
    {
        postExposure = newValue;
        UpdateVolumeProfile();
    }

    public void OnContrastValueChanged(float newValue)
    {
        contrast = newValue;
        UpdateVolumeProfile();
    }

    public void Reset()
    {
        postExposure = 0f;
        contrast = 0f;

        postExposureSlider.value = 0;
        contrastSlider.value = 0;
        UpdateVolumeProfile();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("PostExposure", postExposure);
        PlayerPrefs.SetFloat("Contrast", contrast);
        PlayerPrefs.Save();

        savedPostExposure = postExposure;
        savedContrast = contrast;
    }

    public void ApplySavedValues()
    {	
        if (colorAdjustments == null)
        {
            if (!volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments)) return;
        }

        savedPostExposure = PlayerPrefs.GetFloat("PostExposure", 0f);
        savedContrast = PlayerPrefs.GetFloat("Contrast", 0f);

        postExposure = savedPostExposure;
        contrast = savedContrast;

        postExposureSlider.value = postExposure;
        contrastSlider.value = contrast;

        UpdateVolumeProfile(useSavedValues: true, checkInit: false);
    }
}
