using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(Animator))]
public class DoorController : Lockable
{
    Animator anim;
    NavMeshLink navMeshLink = null;
    bool opened = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
        TryGetComponent<NavMeshLink>(out navMeshLink);

        if (IsOpen())
        {
            anim.SetTrigger("Open");
            if (navMeshLink != null) navMeshLink.activated = true;
        }
        else
        {
            if (navMeshLink != null) navMeshLink.activated = false;
        }
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
            if (navMeshLink != null)
            {
                navMeshLink.activated = opened;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{gameObject.name}: {other.name} entered the trigger");
        GuardianController gc;
        if (other.TryGetComponent(out gc))
        {
            if (gc.CanOpenClosedDoors())
            {
                if (!IsLocked() && !IsOpen())
                {
                    Interact();
                }
            }
        }
    }
}
