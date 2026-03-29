using System;
using UnityEngine;

public class DayCheck : MonoBehaviour
{
    bool April1 = false;
    bool September3 = false;
    bool NewYear = false;
    public static DayCheck Instance = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        //set dates
        DateTime now = DateTime.Now;
        April1 = (now.Month == 4 && now.Day == 1);
        September3 = (now.Month == 9 && now.Day == 3);
        NewYear = (now.Month == 12 && (now.Day >= 25)) || (now.Month == 1 && (now.Day <= 7));
#if UNITY_EDITOR
        // здесь можно принудительно поставить флаги дат дл€ теста
        // April1 = true;
		// September3 = true;
		// NewYear = true;
#endif

        Debug.Log($"ƒата запуска: {now:dd.MM.yy}");
    }

    public bool IsApril1 => April1;
    public bool IsSeptember3 => September3;
    public bool IsNewYear => NewYear;
}
