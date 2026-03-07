using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementMessageController : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text title;
    [SerializeField] Animator animator;
    bool isPlaying = false;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void ShowAchievement(Achievement ach)
    {
        if (isPlaying) return;
        image.sprite = ach.image;
        title.text = ach.title;
        StartCoroutine(ShowAchievementCoroutine());
    }

    IEnumerator ShowAchievementCoroutine()
    {
        isPlaying = true;
        animator.SetTrigger("show");
        yield return new WaitForSeconds(4f);
        isPlaying = false;
    }

    public bool IsPlaying => isPlaying;
}
