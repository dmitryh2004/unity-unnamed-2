using UnityEngine;

public class ArchiveActivateButton : Interactable
{
    [SerializeField] Archive archive;
    public override void Interact()
    {
        archive.OpenArchive();
    }
}
