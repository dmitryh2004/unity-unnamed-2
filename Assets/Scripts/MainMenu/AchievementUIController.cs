using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementUIController : MonoBehaviour
{
    [SerializeField] List<AchievementPanelUIController> panelUI = new();
    [SerializeField] TMP_Text currentPageText;
    [SerializeField] GameObject prevPageBtn, nextPageBtn;
    int currentPage = 1;
    int totalPages = 1;

    private void Start()
    {
        if (AchievementSystem.Instance != null)
        {
            int achievementCount = AchievementSystem.Instance.GetAchievementCount();
            totalPages = (int)Mathf.Ceil((float)achievementCount / panelUI.Count);
            UpdateUI();
        }
        else
        {
            foreach(var panel in panelUI)
            {
                panel.gameObject.SetActive(false);
            }
            currentPageText.text = "Что-то пошло не так :(";
        }
    }

    public void UpdateUI()
    {
        int startIndex = (currentPage-1) * panelUI.Count;
        for (int i = 0; i < panelUI.Count; i++)
        {
            Achievement ach = AchievementSystem.Instance.GetAchievementByIndex(i + startIndex);
            panelUI[i].gameObject.SetActive(ach != null);
            if (ach != null)
            {
                panelUI[i].SetAchievement(ach);
            }
        }

        currentPageText.text = $"Страница {currentPage} из {totalPages}";
        prevPageBtn.SetActive(currentPage > 1);
        nextPageBtn.SetActive(currentPage < totalPages);
    }

    public void PrevPage()
    {
        if (currentPage > 1) currentPage--;
        UpdateUI();
    }

    public void NextPage()
    {
        if (currentPage < totalPages) currentPage++;
        UpdateUI();
    }
}
