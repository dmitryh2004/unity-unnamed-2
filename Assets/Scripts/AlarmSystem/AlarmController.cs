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

        if (AdaptiveDifficultyManager.Instance != null)
        {
            int alertnessDegree = AdaptiveDifficultyManager.Instance.AlertnessDegree;
            float adjustedRemainingTime = timerController.GetRemainingTime();
            adjustedRemainingTime *= (AdaptiveDifficultyManager.Instance.Values.GetParameterValue("ReinforcementTimerMultiplier", alertnessDegree) ?? 1f);
            timerController.SetRemainingTime(adjustedRemainingTime);
        }
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
        if (StatisticCollector.Instance != null)
            StatisticCollector.Instance.AlarmRaised = true;
        if (timerController != null)
            timerController.StartTimer();
        GuardianManager.Instance?.ExpireSpawnTimer();
        FindAnyObjectByType<PlayerCameraBlurController>()?.EnableBlur();
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
