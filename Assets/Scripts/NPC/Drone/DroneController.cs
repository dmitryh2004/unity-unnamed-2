using UnityEngine;
using System.Collections.Generic;

public class DroneController : MonoBehaviour
{
    private int phase = 1; // 1 - неактивен, 2 - взлетает, 3 - летит
    [SerializeField] private float flyHeight = 60f;
    [SerializeField] private float landedHeight = 0f; // написать высоту в неактивном состоянии
    [SerializeField] private float takeOffSpeed = 2f; // скорость взлета
    [SerializeField] float flyingSpeed = 3f;
    [SerializeField] float changeDestinationPointDistance = 1f;
    [SerializeField] float rotationSpeed = 3f; // Скорость поворота в градусах
    [SerializeField] float rotationThreshold = 1f;
    [SerializeField] float maxRotationDiffForMoving = 15f; // Максимальная разница вращения, при которой дрон будет двигаться
    [SerializeField] List<Transform> patrolPoints = new ();
    [SerializeField] List<DroneSeeker> seekers = new ();
    [SerializeField] DroneAudioController audioController;
    private float currentFlyingHeight;
    private int currentPoint = -1;
    private Vector3 destination;
    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        SwitchPhase(1);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        AddTargetToSeekers(player.transform);
    }

    public void AddTargetToSeekers(Transform target)
    {
        foreach(DroneSeeker ds in seekers)
        {
            ds.AddTrackedObject(target);
        }
    }

    public void RemoveTargetFromSeekers(Transform target)
    {
        foreach (DroneSeeker ds in seekers)
        {
            ds.RemoveTrackedObject(target);
        }
    }

    void Start()
    {
        // Launch();
    }

    private void FixedUpdate()
    {
        if (phase == 3 && destination != null)
        {
            Vector3 direction = destination - transform.position;
            direction.y = 0;

            Vector3 forward = transform.forward;
            float angle = Vector3.SignedAngle(forward, direction, Vector3.up);

            if (Mathf.Abs(angle) <= maxRotationDiffForMoving)
                rb.linearVelocity = direction.normalized * flyingSpeed;
            else
                rb.linearVelocity = Vector3.zero;

            if (Mathf.Abs(angle) >= rotationThreshold)
            {
                rb.angularVelocity = new Vector3(0f, Mathf.Sign(angle) * rotationSpeed * Mathf.Deg2Rad, 0f);
            }
            else
            {
                rb.angularVelocity = Vector3.zero;
                //rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y - angle, rb.rotation.eulerAngles.z);
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        rb.position = new Vector3(rb.position.x, currentFlyingHeight, rb.position.z);
    }

    void Update()
    {
        if (phase == 3 && GetDistanceToCurrentPoint() < changeDestinationPointDistance)
        {  
            UpdateDestination();
            print($"Current destination: {destination}");
        }
        UpdateFlyingHeight();
    }

    void UpdateFlyingHeight()
    {
        if (phase == 1)
        {
            currentFlyingHeight = landedHeight;
        }
        else if (phase == 3)
        {
            currentFlyingHeight = flyHeight;
        }
        else
        {
            currentFlyingHeight += Time.deltaTime * takeOffSpeed;
            if (currentFlyingHeight >= flyHeight)
            {
                SwitchPhase(3);
                currentFlyingHeight = flyHeight;
            }
        }
    }

    float GetDistanceToCurrentPoint()
    {
        return (destination != null) ? Vector3.Distance(transform.position, destination) : float.PositiveInfinity;
    }

    void UpdateDestination() 
    {
        if (patrolPoints.Count == 0) return;
        currentPoint++;
        if (currentPoint == patrolPoints.Count) currentPoint = 0;

        destination = patrolPoints[currentPoint].position;
    }

    void SwitchPhase(int newPhase)
    {
        phase = newPhase;
        animator.SetInteger("phase", phase);
        foreach (DroneSeeker seeker in seekers)
        {
            if (phase != 1)
            {
                seeker.Activate();
                audioController.Activate();
            }
            else
            {
                seeker.Deactivate();
                audioController.Deactivate();
            }
        }
        switch(phase)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                currentPoint = -1;
                UpdateDestination();
                break;
        }
    }

    public void Launch()
    {
        SwitchPhase(2);
    }
}
