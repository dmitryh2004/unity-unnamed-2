using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{
    int phase = 1;
    
    //Phase 1
    bool goForward = true;
    int currentPoint = 0;
    float checkPointsTimer = 0f;

    [Header("Links")]
    [SerializeField] Transform eye;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] List<Transform> trackedObjects = new();

    [Header("Common Settings")]
    [SerializeField] float speed = 2f;
    [SerializeField] float runningSpeed = 6f;
    [SerializeField] float spotDistance = 10f;
    [SerializeField] float fov = 60f;
    [SerializeField] float checkPointsInterval = .5f;

    [Header("Phase 1 Settings")]
    [SerializeField] List<Transform> patrolPoints = new();
    [SerializeField] bool delayOnPoints = true;
    [SerializeField] float delay = 7.5f;
    [SerializeField] float turnAroundInterval = 2.5f;

    public bool IsPointVisible(Transform point)
    {
        Vector3 direction = point.position - eye.position;
        if (direction.magnitude > spotDistance) return false;

        Vector3 facingDirection = eye.forward;
        float dot = Vector3.Dot(facingDirection.normalized, direction.normalized);

        if (dot > Mathf.Cos(fov)) return false;

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
                break;
            case 2:
                agent.speed = runningSpeed;
                break;
            case 3:
                agent.speed = speed;
                break;
        }
    }

    private void Update()
    {
        switch (phase)
        {
            case 1:
                checkPointsTimer += Time.deltaTime;
                if (checkPointsTimer >= checkPointsInterval)
                {
                    checkPointsTimer = 0f;
                    Phase1Update();
                }
                break;
        }
    }

    private void Phase1Update()
    {
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
    }
}
