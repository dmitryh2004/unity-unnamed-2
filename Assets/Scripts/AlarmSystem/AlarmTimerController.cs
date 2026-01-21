using UnityEngine;

public class AlarmTimerController : TimerController
{
    protected override void OnExpired()
    {
        base.OnExpired();
        LevelManager.Instance.GameOver(2);
    }
}
