using UnityEngine;

public class ExitDoorController : Interactable
{
    public override void Interact()
    {
        LevelManager.Instance.GameOver(0);
    }
}
