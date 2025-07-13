using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceshipLeverController : Interactable
{
    Animator animator;

    public override void Interact()
    {
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

        LevelManager.Instance.BaseGameOver();
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SpaceshipController.Instance.GetCurrentComplex().sceneName);
    }
}
