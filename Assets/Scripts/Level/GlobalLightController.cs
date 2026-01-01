using UnityEngine;

public class GlobalLightController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Light lightComponent;
    [SerializeField] Terrain terrain;
    [SerializeField] float minYPosition = 0f;
    [SerializeField] bool useTerrain = false;
    bool lightEnabled = true;
    float lightIntensity = 0f;

    private void Start()
    {
        lightIntensity = lightComponent.intensity;
    }

    private void Update()
    {
        float height = (useTerrain) ? terrain.SampleHeight(player.position) + terrain.transform.position.y : minYPosition;
        bool cond = player.position.y >= height;
        
        if (cond != lightEnabled)
        {
            lightEnabled = cond;
            lightComponent.intensity = cond ? lightIntensity : 0f;
        }
    }
}
