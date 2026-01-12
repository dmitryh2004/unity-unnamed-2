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
        Vector3 playerPos = player.position;
        float height = (useTerrain) ? terrain.SampleHeight(playerPos) + terrain.transform.position.y : minYPosition;
        bool cond = player.position.y >= height;
        
        if (useTerrain && !cond)
        {
            bool hasHole = terrain.terrainData.IsHole((int)(playerPos.x - terrain.transform.position.x), (int)(playerPos.z - terrain.transform.position.z));
            cond = cond || hasHole;
        }

        if (cond != lightEnabled)
        {
            lightEnabled = cond;
            lightComponent.intensity = cond ? lightIntensity : 0f;
        }
    }
}
