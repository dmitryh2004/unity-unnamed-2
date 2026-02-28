using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPanelUIController : MonoBehaviour
{
    [SerializeField] Image achievementImage;
    [SerializeField] TMP_Text title, text;
    [SerializeField] TMP_Text progressBarText;
    [SerializeField] Image progressBarImage;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] GameObject progressBarContainer;
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
            achievementImage.sprite = null;
            title.text = "Неизвестно";
            text.text = "Неизвестно";
            progressBarContainer.SetActive(false);
        }
        else
        {
            achievementImage.sprite = currentAchievement.image;
            title.text = currentAchievement.title;
            text.text = currentAchievement.desc;
            if (currentAchievement.hasProgressBar || currentAchievement.hasGoalValue)
            {
                progressBarContainer.SetActive(true);
                int progressBarValue = currentAchievement.hasProgressBar ? currentAchievement.progressBarValue : currentAchievement.goalValue;
                int value = PlayerPrefs.GetInt($"Achievement_{currentAchievement.id}_Progress", 0);
                float ratio = (float)value / progressBarValue;

                progressBarText.text = $"{value} / {progressBarValue}";
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
