using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmController : MonoBehaviour
{
    public static AlarmController Instance = null;
    [SerializeField] AudioSource alarmSoundSource;
    List<AlarmLightController> alarmLights = new();
    [SerializeField] AlarmTimerController timerController;
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
    }

    public AlarmTimerController GetTimerController() => timerController;

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
        StatisticCollector.Instance.AlarmRaised = true;
        if (timerController != null)
            timerController.StartTimer();
        alarmSoundSource.Play();
    }

    public void StopAlarm()
    {
        alarmed = false;
        foreach (AlarmLightController alarmLightController in alarmLights)
        {
            alarmLightController.ChangeState(false);
        }
        if (timerController != null)
            timerController.StopTimer();
        alarmSoundSource.Stop();
    }
}
