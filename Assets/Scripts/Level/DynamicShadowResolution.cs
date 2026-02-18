using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicShadowResolution : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float midDistance = 20f;
    [SerializeField] float minDistance = 30f;

    Light lightComponent;
    LightShadowResolution currentResolution;

    private void OnEnable()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        lightComponent = GetComponent<Light>();

        if (lightComponent == null || player == null)
        {
            enabled = false;
            return;
        }

        currentResolution = LightShadowResolution.High;
        lightComponent.shadowResolution = currentResolution;
        InvokeRepeating(nameof(UpdateResolution), 0f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void UpdateResolution()
    {
        float distance = (transform.position - player.position).magnitude;
        if (distance < midDistance) currentResolution = LightShadowResolution.High;
        else if (distance < minDistance) currentResolution = LightShadowResolution.Medium;
        else currentResolution = LightShadowResolution.Low;
        Debug.Log($"Changed light resolution: distance={distance}, resolution={currentResolution}");
        lightComponent.shadowResolution = currentResolution;
    }
}
