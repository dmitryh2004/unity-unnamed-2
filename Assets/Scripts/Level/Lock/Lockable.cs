using UnityEngine;

public abstract class Lockable : Interactable
{
    [SerializeField] bool locked = false;
    [SerializeField] LockController locker;

    private void Start()
    {
        if (locker != null) locked = true;
    }

    void SetLocked(bool locked)
    {
        this.locked = locked;
    }

    public bool IsLocked()
    {
        return locked;
    }

    public void UpdateLocked()
    {
        if (locker == null)
        {
            SetLocked(false);
        }
        else
        {
            SetLocked(locker.IsActive());
        }
    }
}
