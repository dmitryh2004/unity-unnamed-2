using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{
    int phase = 1;
    float phaseUpdateTimer = 0f;

    //Phase 1
    bool goForward = true;
    int currentPoint = 0;

    //Phase 2
    Transform target = null;

    [Header("Links")]
    [SerializeField] Transform eye;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] List<Transform> trackedObjects = new();

    [Header("Common Settings")]
    [SerializeField] float speed = 2f;
    [SerializeField] float runningSpeed = 6f;
    [SerializeField] float spotDistance = 10f;
    [SerializeField] float fov = 60f;
    [SerializeField] float phaseUpdateInterval = .5f;

    [Header("Phase 1 Settings")]
    [SerializeField] List<Transform> patrolPoints = new();
    [SerializeField] bool delayOnPoints = true;
    [SerializeField] float delay = 7.5f;
    [SerializeField] float turnAroundInterval = 2.5f;

    public bool IsPointVisible(Transform point)
    {
        Vector3 direction = point.position - eye.position;
        Debug.Log($"dir: {point.position} - {eye.position} = {direction}");
        if (direction.magnitude > spotDistance) return false;

        Vector3 facingDirection = eye.forward;
        Debug.Log($"face: {facingDirection}");
        float dot = Vector3.Dot(facingDirection.normalized, direction.normalized);

        Debug.Log($"dot: {dot}");
        if (dot > Mathf.Cos(fov * Mathf.Deg2Rad)) return false;

        RaycastHit hit;
        if (Physics.Raycast(eye.position, direction, out hit, spotDistance))
        {
            if (hit.collider.transform == point) return true;
        }
        return false;
    }

    private void Start()
    {
        SwitchPhase(1);
    }

    public void SwitchPhase(int newPhase)
    {
        phase = newPhase; //change phase
        switch (phase) //update npc
        {
            case 1:
                agent.speed = speed;

                agent.SetDestination(patrolPoints[0].position);
                currentPoint = 0;
                goForward = true;
                break;
            case 2:
                agent.speed = runningSpeed;

                agent.SetDestination(target.position);
                break;
            case 3:
                agent.speed = speed;
                break;
        }
    }

    private void Update()
    {
        phaseUpdateTimer += Time.deltaTime;
        if (phaseUpdateTimer >= phaseUpdateInterval)
        {
            phaseUpdateTimer = 0f;
            switch (phase)
            {
                case 1:
                    Phase1Update();
                    break;
                case 2:
                    Phase2Update();
                    break;
            }
        }
    }

    private void Phase1Update()
    {
        // update npc movement
        if (agent.destination != null)
        {
            if (agent.remainingDistance < 0.5f)
            {
                currentPoint += ((goForward) ? 1 : -1);
                if (currentPoint == 0 || currentPoint == patrolPoints.Count - 1) goForward = !goForward;

                agent.SetDestination(patrolPoints[currentPoint].position);
            }
        }
        else
        {
            agent.SetDestination(patrolPoints[0].position);
            currentPoint = 0;
            goForward = true;
        }

        // check for tracked objects
        foreach (var tracked in trackedObjects)
        {
            if (IsPointVisible(tracked))
            {
                target = tracked;
                SwitchPhase(2);
                break;
            }
            else
            {
                Debug.Log(gameObject.name + ": target " + tracked + " is not visible");
            }
        }
    }

    private void Phase2Update()
    {
        // check for player in 0.5 range
        
        // update npc movement
        if (agent.destination != null)
        {
            if (agent.remainingDistance < 0.5f)
            {
                SwitchPhase(1);
            }
        }

        // update npc destination if target is visible
        if (target != null)
        {
            if (IsPointVisible(target))
            {
                agent.SetDestination(target.position);
            }
        }
        else
        {
            SwitchPhase(1);
        }
    }
}
