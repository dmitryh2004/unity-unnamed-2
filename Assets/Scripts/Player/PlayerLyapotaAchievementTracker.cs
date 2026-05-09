using UnityEngine;

public class PlayerLyapotaAchievementTracker : MonoBehaviour
{
    [SerializeField] float requiredTime = 60f;
    [SerializeField] float minHeight = 50f;
    float timer = 0f;

    void Update() 
    {
        if ((!AchievementSystem.Instance?.IsAchievementAchieved("lyapota")) ?? false) {
            if (transform.position.y >= 50f) {
                timer += Time.deltaTime;
                if (timer > requiredTime) {
                    AchievementActionTracker.Instance?.OnLyapotaChanged(timer);
                }
            }
            else {
                AchievementActionTracker.Instance?.OnLyapotaChanged(timer);
                timer = 0f;
            }
        }
    }
}
