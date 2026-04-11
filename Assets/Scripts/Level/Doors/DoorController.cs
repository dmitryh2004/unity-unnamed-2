using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;

//[RequireComponent(typeof(Animator))]
public class DoorController : Lockable
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshLink navMeshLink = null;
    [Tooltip("Название двери (дверь, ящик, сейф и т.д.) в винительном падеже (открыть что?)")]
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

    [Space]
    [Header("Trigger room events")]
    [SerializeField] bool triggerOpenDoorEvent = false;
    [SerializeField] bool triggerOpenSafeEvent = false;
    [SerializeField] bool triggerOpenTableEvent = false;
    bool firstOpen = true;

    [Space]
    [SerializeField] bool checkTriggerZone = false; // не закрывать, если игрок в триггер-коллайдере
    bool inTriggerZone = false;

    public bool CheckTriggerZone => checkTriggerZone;
    public bool InTriggerZone => inTriggerZone;
    private void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (navMeshLink == null) TryGetComponent<NavMeshLink>(out navMeshLink);

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

    public void SetInTriggerZone(bool value) => this.inTriggerZone = value;

    public void ChangeDoorState(bool opened)
    {
        if (!IsLocked() && !(checkTriggerZone && inTriggerZone))
        {
            this.opened = opened;
            if (this.opened)
            {
                if (firstOpen)
                {
                    firstOpen = false;
                    RoomEventManager rem = GetComponentInParent<RoomEventManager>();
                    if (rem != null)
                    {
                        if (triggerOpenDoorEvent) rem.FirstDoorOpenedEvent();
                        if (triggerOpenSafeEvent) rem.SafeOpenedEvent();
                        if (triggerOpenTableEvent) rem.TableOpenedEvent();
                    }
                }
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
}
