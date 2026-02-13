using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DoorController : Lockable
{
    Animator anim;
    NavMeshLink navMeshLink = null;
    [Tooltip("Ќазвание двери (дверь, €щик, сейф и т.д.) в винительном падеже (открыть что?)")]
    [SerializeField] string doorName = "дверь";
    [SerializeField] bool canBeOpenedManually = true;
    bool opened = false;

    [Space]
    [SerializeField] float openDoorDuration = 1f;
    [SerializeField] float closeDoorDuration = 1f;
    public float OpenDoorDuration => openDoorDuration;
    public float CloseDoorDuration => closeDoorDuration;

    [Space]
    [SerializeField] DoorAudioPlayer audioPlayer;
    [SerializeField] bool playCreakAudio = true;
    private void Start()
    {
        anim = GetComponent<Animator>();
        TryGetComponent<NavMeshLink>(out navMeshLink);

        if (IsOpen())
        {
            anim.SetTrigger("Open");
        }
        if (navMeshLink != null) navMeshLink.activated = !IsLocked();
    }

    public string GetName() => doorName;

    public bool IsOpen()
    {
        return !IsLocked() && opened;
    }

    public bool CanBeOpenedManually() => canBeOpenedManually;

    public override void Interact()
    {
        if (canBeOpenedManually)
        {
            ChangeDoorState(!opened);
        }
    }

    public void ChangeDoorState(bool opened)
    {
        if (!IsLocked())
        {
            this.opened = opened;
            if (this.opened)
            {
                anim.SetTrigger("Open");
                if (audioPlayer != null)
                    StartCoroutine(PlayOpenAudios());
            }
            else
            {
                anim.SetTrigger("Close");
                if (audioPlayer != null)
                    StartCoroutine(PlayCloseAudios());
            }
        }
        if (navMeshLink != null) navMeshLink.activated = !IsLocked();
    }

    IEnumerator PlayOpenAudios()
    {
        audioPlayer.PlayOpenStartAudio();
        audioPlayer.PlayOpenAudio();
        yield return new WaitForSeconds(openDoorDuration);
        if (playCreakAudio)
            audioPlayer.PlayOpenEndAudio();
    }
    IEnumerator PlayCloseAudios()
    {
        if (playCreakAudio)
        {
            audioPlayer.PlayCloseStartAudio();
            audioPlayer.PlayCloseAudio();
        }
        yield return new WaitForSeconds(closeDoorDuration);
        audioPlayer.PlayCloseEndAudio();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{gameObject.name}: {other.name} entered the trigger");
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
