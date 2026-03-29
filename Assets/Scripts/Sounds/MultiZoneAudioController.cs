using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MultiZoneAudioController : MonoBehaviour
{
    [SerializeField] private Transform listener;        // слушатель (камера/игрок)
    [SerializeField] private float defaultVolume = 0f; // громкость вне всех зон

    private AudioSource audioSource;
    private Dictionary<Collider, float> zoneVolumes = new Dictionary<Collider, float>();
    private HashSet<Collider> insideZones = new HashSet<Collider>();

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // собираем все триггер‑коллайдеры‑дети как зоны
        Collider[] childTriggers = GetComponentsInChildren<Collider>();

        for (int i = 0; i < childTriggers.Length; i++)
        {
            Collider col = childTriggers[i];
            if (col.isTrigger)
            {
                zoneVolumes[col] = col.CompareTag("AudioZone") ? (col.gameObject.GetComponent<AudioZone>()?.volume ?? 1f) : 1f;
                //Debug.Log($"Zone {i}: volume={zoneVolumes[col]}");
            }
        }

        UpdateVolume();
    }

    public void ReportZoneEnter(Collider childCollider)
    {
        if (zoneVolumes.ContainsKey(childCollider))
        {
            insideZones.Add(childCollider);
            //Debug.Log($"Entered zone with volume={zoneVolumes[childCollider]}");
            UpdateVolume();
        }
    }

    public void ReportZoneExit(Collider childCollider)
    {
        if (insideZones.Remove(childCollider))
        {
            //Debug.Log($"Left zone with volume={zoneVolumes[childCollider]}");
            UpdateVolume();
        }
    }

    void UpdateVolume()
    {
        float maxVolume = -1;

        foreach (Collider zone in insideZones)
        {
            if (zoneVolumes.TryGetValue(zone, out float vol))
            {
                maxVolume = Mathf.Max(maxVolume, vol);
                //Debug.Log($"Listener in zone with volume={vol}, maxVolume={maxVolume}");
            }
        }
        //Debug.Log($"Calculated max volume={maxVolume}");
        audioSource.volume = (maxVolume != -1) ? maxVolume : defaultVolume;
    }
}
