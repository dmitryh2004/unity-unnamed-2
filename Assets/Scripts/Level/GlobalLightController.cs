using UnityEngine;

public class GlobalLightController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Light lightComponent;
    [SerializeField] float minYPosition = 0f;
    bool lightEnabled = true;
    float lightIntensity = 0f;

    private void Start()
    {
        lightIntensity = lightComponent.intensity;
    }

    private void Update()
    {
        bool cond = player.position.y >= minYPosition;
        if (cond != lightEnabled)
        {
            lightEnabled = cond;
            lightComponent.intensity = cond ? lightIntensity : 0f;
        }
    }
}
