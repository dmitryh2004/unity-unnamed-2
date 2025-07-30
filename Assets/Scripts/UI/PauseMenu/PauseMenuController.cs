using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [Space]
    [SerializeField] GameObject exitMainMenuConfirmation;
    [SerializeField] GameObject exitGameConfirmation;
    [Space]
    [SerializeField] ExitGame exitGameController;
    bool visible = false;
    bool exitMainMenuConfirmationVisible = false;
    bool exitGameConfirmationVisible = false;

    void UpdateWindow()
    {
        pauseMenu.SetActive(visible);
        exitMainMenuConfirmation.SetActive(exitMainMenuConfirmationVisible);
        exitGameConfirmation.SetActive(exitGameConfirmationVisible);
    }

    public void Pause()
    {
        ShowWindow();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        HideWindow();
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    public void ExitGame()
    {
        exitGameController.Exit();
    }

    public void ShowWindow()
    {
        visible = true;
        UpdateWindow();
    }

    public void HideWindow()
    {
        visible = false;
        UpdateWindow();
    }

    public void ShowExitMainMenuConfirmation()
    {
        exitMainMenuConfirmationVisible = true;
        UpdateWindow();
    }

    public void HideExitMainMenuConfirmation()
    {
        exitMainMenuConfirmationVisible = false;
        UpdateWindow();
    }

    public void ShowExitGameConfirmation()
    {
        exitGameConfirmationVisible = true;
        UpdateWindow();
    }

    public void HideExitGameConfirmation()
    {
        exitGameConfirmationVisible = false;
        UpdateWindow();
    }
}
