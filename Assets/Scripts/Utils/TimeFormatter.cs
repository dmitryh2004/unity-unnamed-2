using UnityEngine;

public static class TimeFormatter
{
    public static string GetTime(float totalSeconds)
    {
        int minutes = ((int)totalSeconds) / 60;
        int seconds = ((int)totalSeconds) % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
}
