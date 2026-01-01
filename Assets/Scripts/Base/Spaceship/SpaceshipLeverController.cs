using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceshipLeverController : Interactable
{
    Animator animator;
    bool playerDefeated;

    public bool CheckTakeOffConditions()
    {
        return HasUncompletedOrder() && IsEnoughMoney();
    }

    public bool HasUncompletedOrder() => QuotaSystem.Instance.HasUncompletedOrder();
    public bool IsEnoughMoney() => PlayerWallet.Instance.CanAfford(SpaceshipController.Instance.GetCurrentComplex().cost);

    public override void Interact()
    {
        if (CheckTakeOffConditions())
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

        int cost = SpaceshipController.Instance.GetCurrentComplex().cost;
        if (cost > 0)
        {
            PlayerWallet.Instance.SubtractMoney(cost);
        }
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
