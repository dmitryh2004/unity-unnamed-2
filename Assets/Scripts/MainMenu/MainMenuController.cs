using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuUI, aboutUI;
    int currentUI = 0; // 0 - mainMenu, 1 - about
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BaseScene");
    }
}
