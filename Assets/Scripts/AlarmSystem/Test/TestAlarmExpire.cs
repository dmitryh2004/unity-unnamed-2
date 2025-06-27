using UnityEngine;

public class TestAlarmExpire : MonoBehaviour
{
    private void Start()
    {
        AlarmTimerController.OnExpire += ExitGame;
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}
