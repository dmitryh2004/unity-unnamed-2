using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TimedColor
{
    public int minSeconds, maxSeconds;
    public Color color;
}
public class AlarmTimerController : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] float startSeconds = 600f;
    [SerializeField] List<TimedColor> colors = new();
    float secondsRemaining;
    bool playing = false;

    public bool IsPlaying() => playing;
    public float GetRemainingTime() => secondsRemaining;

    public string GetTimerText(bool internalUsage = true)
    {
        int minutes = ((int)secondsRemaining) / 60;
        int seconds = ((int)secondsRemaining) % 60;
        
        if (playing && (seconds % 2 == 1))
            return $"{minutes:D2}{(internalUsage ? " " : ":")}{seconds:D2}";
        return $"{minutes:D2}:{seconds:D2}";
    }

    Color GetTimerColor()
    {
        foreach (TimedColor tc in colors)
        {
            if (tc.minSeconds <= secondsRemaining && secondsRemaining < tc.maxSeconds)
            {
                return tc.color;
            }
        }
        return Color.white;
    }

    public void StartTimer()
    {
        playing = true;
    }

    public void StopTimer()
    {
        playing = false;
    }

    private void Start()
    {
        secondsRemaining = startSeconds;
        timerText.text = GetTimerText();
    }

    private void Update()
    {
        if (playing)
        {
            secondsRemaining -= Time.deltaTime;
            if (secondsRemaining < 0f)
            {
                secondsRemaining = 0f;
                StopTimer();
                LevelManager.Instance.GameOver(2);
            }
            timerText.text = GetTimerText();
            timerText.color = GetTimerColor();
        }
    }
}
