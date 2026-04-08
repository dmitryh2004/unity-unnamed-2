using UnityEngine;
using UnityEngine.Rendering;

public class LightSourceCuller : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Light thisLight;
    public float checkInterval = 0.5f; // Проверять каждые 0.5 сек для производительности

    [Header("Light Properties")]
    public float lightRange = 10f;

    private Renderer selfRenderer;
    private Renderer[] nearbyRenderers;
    private float lastCheckTime;
    private bool wasEnabled;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (thisLight == null)
            thisLight = GetComponent<Light>();

        selfRenderer = GetComponent<Renderer>();
        wasEnabled = thisLight.enabled;
        InvokeRepeating(nameof(CheckVisibility), 0f, checkInterval);
    }

    void CheckVisibility()
    {
        bool shouldEnable = IsLightVisibleToCamera() || AreLitSurfacesVisible();

        lightRange = 2 * thisLight.range;

        if (shouldEnable != thisLight.enabled)
        {
            thisLight.enabled = shouldEnable;
        }
    }

    bool IsLightVisibleToCamera()
    {
        if (playerCamera == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            print($"light {name} - planes: {planes}, bounds: {col.bounds}");
            return GeometryUtility.TestPlanesAABB(planes, col.bounds);
        }
        else
        {
            // Fallback: расстояние до камеры
            return Vector3.Distance(transform.position, playerCamera.transform.position) < lightRange;
        }
    }

    bool AreLitSurfacesVisible()
    {
        Collider lightCol = GetComponent<Collider>();
        if (lightCol == null) return false;

        // Находим nearby объекты с Renderer в радиусе света
        Collider[] hits = Physics.OverlapSphere(transform.position, lightRange, LayerMask.GetMask("Default")); // Укажите нужные слои
        nearbyRenderers ??= new Renderer[0];

        System.Collections.Generic.List<Renderer> candidates = new();
        foreach (var hit in hits)
        {
            Renderer rend = hit.GetComponent<Renderer>();
            if (rend != null && rend != selfRenderer)
            {
                candidates.Add(rend);
            }
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        foreach (Renderer rend in candidates)
        {
            print($"light {name} - planes: {planes}, bounds: {rend.bounds}");
            if (GeometryUtility.TestPlanesAABB(planes, rend.bounds))
            {
                return true;
            }
        }
        return false;
    }
}
