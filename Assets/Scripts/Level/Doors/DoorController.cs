using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : Interactable
{
    Animator anim;
    bool opened = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool IsOpen()
    {
        return opened;
    }

    public override void Interact()
    {
        opened = !opened;
        if (opened) anim.SetTrigger("Open"); else anim.SetTrigger("Close");
    }
}
