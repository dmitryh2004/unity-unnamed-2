using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{
    System.Random random = new();

    int phase = 1;
    float phaseUpdateTimer = 0f;

    //Phase 1
    bool goForward = true;
    int currentPoint = 0;

    //Phase 2
    Transform target = null;

    //Phase 3
    float phase3Timer = 0f;
    float phase3TurnAroundTimer = 0f;

    [Header("Links")]
    [SerializeField] Transform eye;
    [SerializeField] Light fovLight;
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
    [SerializeField] bool enterPhase3OnPoints = true;

    [Header("Phase 2 Settings")]
    [SerializeField] bool openClosedDoors = true;

    [Header("Phase 3 Settings")]
    [SerializeField] float delay = 7.5f;
    [SerializeField] float turnAroundInterval = 2.5f;

    public bool CanOpenClosedDoors() => openClosedDoors;

    public bool IsPointVisible(Transform point)
    {
        Vector3 direction = point.position - eye.position;
        //Debug.Log($"dir: {point.position} - {eye.position} = {direction}");
        if (direction.magnitude > spotDistance) return false;

        Vector3 facingDirection = eye.forward;
        //Debug.Log($"face: {facingDirection}");
        float dot = Vector3.Dot(facingDirection.normalized, direction.normalized);

        //Debug.Log($"dot: {dot}");
        if (dot < Mathf.Cos(fov * Mathf.Deg2Rad)) return false;

        RaycastHit hit;
        if (Physics.Raycast(eye.position, direction, out hit, spotDistance))
        {
            if (hit.collider.transform == point) return true;
        }
        return false;
    }

    private void Start()
    {
        fovLight.range = spotDistance;
        fovLight.spotAngle = fov * 2;
        SwitchPhase(1);
    }

    public void SwitchPhase(int newPhase)
    {
        phase = newPhase; //change phase
        switch (phase) //update npc
        {
            case 1:
                agent.speed = speed;
                SetNextWaypoint();
                break;
            case 2:
                agent.speed = runningSpeed;

                agent.SetDestination(target.position);
                break;
            case 3:
                agent.speed = speed;

                phase3Timer = 0f;
                phase3TurnAroundTimer = 0f;
                break;
        }
    }

    private void Update()
    {
        phaseUpdateTimer += Time.deltaTime;
        if (phase == 3)
        {
            phase3Timer += Time.deltaTime;
            phase3TurnAroundTimer += Time.deltaTime;
        }
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
                case 3:
                    Phase3Update();
                    break;
            }
        }
    }

    private void SetNextWaypoint()
    {
        currentPoint += ((goForward) ? 1 : -1);
        if (currentPoint == 0 || currentPoint == patrolPoints.Count - 1) goForward = !goForward;

        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    private void CheckForTrackedObjects()
    {
        // check for tracked objects
        foreach (var tracked in trackedObjects)
        {
            if (IsPointVisible(tracked))
            {
                target = tracked;
                SwitchPhase(2);
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
                if (enterPhase3OnPoints)
                {
                    SwitchPhase(3);
                }
                else
                {
                    SetNextWaypoint();
                }
            }
        }
        else
        {
            agent.SetDestination(patrolPoints[0].position);
            currentPoint = 0;
            goForward = true;
        }

        CheckForTrackedObjects();
    }

    private void Phase2Update()
    {
        // check for player in 0.5 range
        
        // update npc movement
        if (agent.destination != null)
        {
            if (agent.remainingDistance < 0.5f) // if npc is near the dest point, switch phase to 3
            {
                SwitchPhase(3);
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
            SwitchPhase(3);
        }
    }

    private void Phase3Update()
    {
        if (phase3Timer >= delay) // exit to phase 1
        {
            SwitchPhase(1);
            return;
        }

        // handle turnaround
        if (phase3TurnAroundTimer >= turnAroundInterval)
        {
            phase3TurnAroundTimer = 0f;
            float randomAngle = 4 * fov * (float)(random.NextDouble() - 0.5);
            transform.Rotate(new Vector3(0f, randomAngle, 0f));
        }

        CheckForTrackedObjects();
    }
}
