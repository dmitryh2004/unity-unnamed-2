using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : Interactable
{
    Animator anim;
    bool opened = false;
    [SerializeField] bool locked = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool IsOpen()
    {
        return !locked && opened;
    }

    public override void Interact()
    {
        if (!locked)
        {
            opened = !opened;
            if (opened) anim.SetTrigger("Open"); else anim.SetTrigger("Close");
        }
    }

    public bool IsLocked()
    {
        return locked;
    }
}
