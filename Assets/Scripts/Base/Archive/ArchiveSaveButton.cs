using UnityEngine;

public class ArchiveSaveButton : Interactable
{
    [SerializeField] Archive archive;
    public override void Interact()
    {
        archive.SaveGame();
    }
}
