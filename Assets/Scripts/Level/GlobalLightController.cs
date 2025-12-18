using UnityEngine;

public class GlobalLightController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Light lightComponent;
    [SerializeField] float minYPosition = 0f;
    bool lightEnabled = true;

    private void Update()
    {
        bool cond = player.position.y >= minYPosition;
        if (cond != lightEnabled)
        {
            lightComponent.enabled = cond;
            lightEnabled = cond;
        }
    }
}
