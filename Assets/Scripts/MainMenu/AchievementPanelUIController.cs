using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AchievementCategoryColor
{
    public AchievementCategory category = AchievementCategory.Other;
    public Color color = Color.black;
}

public class AchievementPanelUIController : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] Image achievementImage, achievementCompletedImage;
    Material materialInstance;
    [SerializeField] TMP_Text title, text;
    [SerializeField] TMP_Text progressBarText;
    [SerializeField] Image progressBarImage;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] GameObject progressBarContainer;
    [SerializeField] List<AchievementCategoryColor> achievementCategoryColors = new();
    Achievement currentAchievement = null;

    private void Awake()
    {
        materialInstance = new Material(achievementImage.material);
        achievementImage.material = materialInstance;
    }

    public void SetAchievement(Achievement achievement)
    {
        currentAchievement = achievement;
        if (currentAchievement != null)
        {
            UpdateUI();
        }
    }

    void SetSprite(Sprite sprite)
    {
        materialInstance.SetTexture("_MainTex", sprite.texture);
    }
    void SetGrayscaled(bool grayscaled)
    {
        materialInstance.SetFloat("_grayscaled", grayscaled ? 1 : 0);
    }

    public void UpdateUI()
    {
        if (currentAchievement == null)
        {
            panel.color = achievementCategoryColors.Find((x) => x.category == AchievementCategory.Other)?.color ?? new Color(1f, 1f, 1f, 0.4f);
            achievementImage.sprite = null;
            achievementCompletedImage.gameObject.SetActive(false);
            SetSprite(null);
            SetGrayscaled(false);
            title.text = "Неизвестно";
            text.text = "Неизвестно";
            progressBarContainer.SetActive(false);
        }
        else
        {
            panel.color = achievementCategoryColors.Find((x) => x.category == currentAchievement.categoryID)?.color ?? new Color(1f, 1f, 1f, 0.4f);
            achievementImage.sprite = currentAchievement.image;
            achievementCompletedImage.gameObject.SetActive(AchievementSystem.Instance.IsAchievementAchieved(currentAchievement.id));
            SetSprite(currentAchievement.image);
            SetGrayscaled(!AchievementSystem.Instance.IsAchievementAchieved(currentAchievement.id));

            title.text = currentAchievement.title;
            text.text = currentAchievement.desc;
            if (currentAchievement.hasProgressBar || currentAchievement.hasGoalValue)
            {
                progressBarContainer.SetActive(true);
                int progressBarValue = currentAchievement.targetValue;
                int value = PlayerPrefs.GetInt($"Achievement_{currentAchievement.id}_Progress", 0);
                float ratio = (float)value / progressBarValue;

                progressBarText.text = $"{NumberFormatter.FormatNumberWithGrouping(value)} / {NumberFormatter.FormatNumberWithGrouping(progressBarValue)}";
                progressBarImage.color = (ratio >= 1) ? Color.green : Color.cyan;
                progressBar.SetMaxValue(progressBarValue);
                progressBar.SetProgress(value);
            }
            else
            {
                progressBarContainer.SetActive(false);
            }
        }
    }
}
