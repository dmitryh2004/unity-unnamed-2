using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : Lockable
{
    Animator anim;
    bool opened = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool IsOpen()
    {
        return !IsLocked() && opened;
    }

    public override void Interact()
    {
        if (!IsLocked())
        {
            opened = !opened;
            if (opened) anim.SetTrigger("Open"); else anim.SetTrigger("Close");
        }
    }
}
