using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Archive : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float saveCooldown = 5f;
    [SerializeField] Animator animator;
    [SerializeField] ArchiveUIController uiController;
    [SerializeField] List<Article> articles = new();
    float timer = 0f;
    public void SaveGame()
    {
        if (timer == 0f)
        {
            timer = saveCooldown;
            LevelManager.Instance.SaveGame(showMessage: true);
        }
    }

    public void OpenArchive()
    {
        uiController.ShowWindow();
        uiController.SetArticle(articles[0]);
        animator.SetTrigger("activate");
    }

    public void CloseArchive(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "ArchiveUI")
            CloseArchive();
    }

    public void CloseArchive()
    {
        uiController.HideWindow();
        animator.SetTrigger("deactivate");
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer = Mathf.Clamp(timer - Time.deltaTime, 0f, saveCooldown);
        }
    }
}
