using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuUI, aboutUI;
    int currentUI = 0; // 0 - mainMenu, 1 - about
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BaseScene");
    }

    public void ChangeScreen(int screen)
    {
        currentUI = screen;
        UpdateWindow();
    }

    void UpdateWindow()
    {
        mainMenuUI.SetActive(currentUI == 0);
        aboutUI.SetActive(currentUI == 1);
    }

    private void Start()
    {
        ChangeScreen(0);
    }
}
