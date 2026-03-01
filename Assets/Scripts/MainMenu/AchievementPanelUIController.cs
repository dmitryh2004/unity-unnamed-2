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
    [SerializeField] Image achievementImage;
    [SerializeField] TMP_Text title, text;
    [SerializeField] TMP_Text progressBarText;
    [SerializeField] Image progressBarImage;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] GameObject progressBarContainer;
    [SerializeField] List<AchievementCategoryColor> achievementCategoryColors = new();
    Achievement currentAchievement = null;

    public void SetAchievement(Achievement achievement)
    {
        currentAchievement = achievement;
        if (currentAchievement != null)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (currentAchievement == null)
        {
            panel.color = achievementCategoryColors.Find((x) => x.category == AchievementCategory.Other)?.color ?? new Color(1f, 1f, 1f, 0.4f);
            achievementImage.sprite = null;
            title.text = "Неизвестно";
            text.text = "Неизвестно";
            progressBarContainer.SetActive(false);
        }
        else
        {
            panel.color = achievementCategoryColors.Find((x) => x.category == currentAchievement.categoryID)?.color ?? new Color(1f, 1f, 1f, 0.4f);
            achievementImage.sprite = currentAchievement.image;
            achievementImage.color = AchievementSystem.Instance.IsAchievementAchieved(currentAchievement.id) ? Color.white : Color.gray;

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
                progressBar.SetProgress(value);
                progressBar.SetMaxValue(progressBarValue);
            }
            else
            {
                progressBarContainer.SetActive(false);
            }
        }
    }
}
