using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmController : MonoBehaviour
{
    public static AlarmController Instance = null;
    [SerializeField] AudioSource alarmSoundSource;
    List<AlarmLightController> alarmLights = new();
    bool alarmed = false;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        alarmLights = new List<AlarmLightController>(FindObjectsByType<AlarmLightController>(FindObjectsSortMode.None));

        //StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(5f);
        StartAlarm();
    }

    public bool GetAlarmState()
    {
        return alarmed;
    }

    public void StartAlarm()
    {
        alarmed = true;
        foreach (AlarmLightController alarmLightController in alarmLights)
        {
            alarmLightController.ChangeState(true);
        }

        alarmSoundSource.Play();
    }
}
