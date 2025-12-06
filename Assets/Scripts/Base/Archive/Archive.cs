using UnityEngine;

public class Archive : MonoBehaviour
{
    [SerializeField] float saveCooldown = 5f;
    float timer = 0f;
    public void SaveGame()
    {
        if (timer == 0f)
        {
            timer = saveCooldown;
            LevelManager.Instance.SaveGame(showMessage: true);
        }
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer = Mathf.Clamp(timer - Time.deltaTime, 0f, saveCooldown);
        }
    }
}
