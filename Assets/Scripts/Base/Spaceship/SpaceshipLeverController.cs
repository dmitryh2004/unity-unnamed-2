using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceshipLeverController : Interactable
{
    Animator animator;
    bool playerDefeated;

    public override void Interact()
    {
        if (QuotaSystem.Instance.HasUncompletedOrder())
            StartCoroutine(StartShipCoroutine());
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    IEnumerator StartShipCoroutine()
    {
        InputActionMapSwitcher.Instance.DisableAllMaps();
        animator.SetTrigger("Start");

        QuotaSystem.Instance.SetDaysLeft(QuotaSystem.Instance.GetDaysLeft() - 1);
        playerDefeated = QuotaSystem.Instance.GetDaysLeft() < 0;

        LevelManager.Instance.BaseGameOver();
        yield return new WaitForSeconds(2f);
        
        if (!playerDefeated)
        {
            SceneManager.LoadScene(SpaceshipController.Instance.GetCurrentComplex().sceneName);
        }
        else
        {
            LevelManager.Instance.DeleteSave();
            SceneManager.LoadScene("DefeatScene");
        }
    }
}
