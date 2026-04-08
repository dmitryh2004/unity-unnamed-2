using UnityEngine;

public class AudioZone : MonoBehaviour
{
    public Collider triggerZone;   // BoxCollider / SphereCollider ñ isTrigger=true
    public float volume = 1.0f;
    private MultiZoneAudioController parentController;

    void Awake()
    {
        parentController = GetComponentInParent<MultiZoneAudioController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (parentController != null)
            parentController.ReportZoneEnter(triggerZone);
    }

    void OnTriggerExit(Collider other)
    {
        if (parentController != null)
            parentController.ReportZoneExit(triggerZone);
    }
}
