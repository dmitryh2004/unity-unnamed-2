using System.Collections.Generic;
using UnityEngine;

public class DroneSeeker : MonoBehaviour
{
    [SerializeField] List<Transform> trackedObjects = new ();
    [SerializeField] LineRenderer lineRenderer;
    Material lineRendererMaterial;
    [SerializeField] Transform laserLight;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float range = 150f;
    [SerializeField] float angle = 30f;
    [SerializeField] float seekPeriod = .5f;
    float seekTimer = 0f;
    bool active = false;
    Transform trackingTarget = null;

    private void Start()
    {
        lineRendererMaterial = lineRenderer.material;
        lineRendererMaterial.SetFloat("_StripeOffset", 0f);
    }

    public void AddTrackedObject(Transform newObject)
    {
        if (!trackedObjects.Contains(newObject))
            trackedObjects.Add(newObject);
    }

    public void RemoveTrackedObject(Transform obj)
    {
        if (trackedObjects.Contains(obj))
            trackedObjects.Remove(obj);
    }

    private void Update()
    {
        if (active)
        {
            seekTimer += Time.deltaTime;
            if (seekTimer >= seekPeriod)
            {
                seekTimer = 0f;
                CheckForObjects();
            }
        }
        else
        {
            seekTimer = 0f;
            trackingTarget = null;
        }
        UpdateLineRenderer();
    }

    public bool IsPointVisible(Transform point)
    {
        Vector3 direction = point.position - transform.position;
        //Debug.Log($"dir: {point.position} - {headObj.position} = {direction}");

        if (direction.magnitude > range) return false;

        Vector3 facingDirection = transform.forward;
        //Debug.Log($"face: {facingDirection}");
        float dot = Vector3.Dot(facingDirection.normalized, direction.normalized);

        float minDot = Mathf.Deg2Rad * angle;
        //Debug.Log($"dot: {dot}");
        if (dot < Mathf.Cos(minDot)) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, range, layerMask.value, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log($"{hit.collider.gameObject.name} (pos: {hit.collider.transform.position})");
            if (hit.collider.transform == point) return true;
        }
        return false;
    }

    void UpdateLineRenderer()
    {
        if (trackingTarget != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[] { transform.position, trackingTarget.position });
            lineRendererMaterial.SetFloat("_StripeOffset", seekTimer / seekPeriod);
            lineRendererMaterial.SetVector("_StartPosition", lineRenderer.GetPosition(0));

            laserLight.gameObject.SetActive(true);
            laserLight.position = lineRenderer.GetPosition(1);
        }
        else
        {
            lineRenderer.enabled = false;
            laserLight.gameObject.SetActive(false);
        }
    }

    void CheckForObjects()
    {
        trackingTarget = null;
        if (LevelManager.Instance.IsGameOver) return;
        foreach (Transform obj in trackedObjects)
        {
            if (IsPointVisible(obj))
            {
                if (!AlarmController.Instance.GetAlarmState())
                {
                    AlarmController.Instance.StartAlarm();
                }
                if (obj.CompareTag("Player")) 
                {
                    trackingTarget = obj;
                    GuardianManager.Instance.CallGuardians();
                    AchievementActionTracker.Instance?.OnDroneSpottedPlayer();
                }
            }
        }
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }
}
