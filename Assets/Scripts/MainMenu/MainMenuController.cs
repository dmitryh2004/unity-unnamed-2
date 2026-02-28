using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuUI, aboutUI, selectSlotUI, achievementsUI;
    [SerializeField] MainMenuSlotUIController selectSlotUIController;
    [SerializeField] MainMenuButtonAudioPlayer audioPlayer;
    [SerializeField] ExitGame gameExit;
    int currentUI = 0; // 0 - mainMenu, 1 - about, 2 - selectSlot
    public void StartGame()
    {
        if (selectSlotUIController.GetSelectedSlot() != -1)
        {
            StartCoroutine(_StartGame());
        }
        else
        {
            audioPlayer.PlayClickIncorrectSound();
        }
    }

    IEnumerator _StartGame()
    {
        yield return new WaitForSeconds(audioPlayer.PlayClickSound());
        UnityEngine.SceneManagement.SceneManager.LoadScene("BaseScene");
    }

    public void ExitGame()
    {
        StartCoroutine(_ExitGame());
    }

    IEnumerator _ExitGame()
    {
        yield return new WaitForSeconds(audioPlayer.PlayClickSound());
        gameExit.Exit();
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
        selectSlotUI.SetActive(currentUI == 2);
        achievementsUI.SetActive(currentUI == 3);

        if (currentUI == 2)
        {
            selectSlotUIController.SelectSlot(-1);
        }
    }

    private void Start()
    {
        ChangeScreen(0);
    }
}
