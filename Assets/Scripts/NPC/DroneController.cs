using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DroneController : MonoBehaviour
{
    private int phase = 1; // 1 - неактивен, 2 - взлетает, 3 - летит
    [SerializeField] private float flyHeight = 60f;
    [SerializeField] private float landedHeight = 0f; // написать высоту в неактивном состоянии
    [SerializeField] private float takeOffSpeed = 2f; // скорость взлета
    [SerializeField] float flyingSpeed = 3f;
    [SerializeField] List<Transform> patrolPoints = new ();
    private float currentFlyHeight;
    private int currentPoint = -1;
    private Vector3 destination;

    void Awake()
    {
        phase = 1;
    }

    void Start() {
        Launch();
    }

    void Update()
    {
        if (phase == 3) {  
            UpdateDestination();
        }
        UpdatePosition();
    }

    void UpdatePosition() {
        if (phase == 1) {
            currentFlyHeight = landedHeight;
        }
        else if (phase == 3) {
            currentFlyHeight = flyHeight;
        }
        else {
            currentFlyHeight += Time.deltaTime * takeOffSpeed;
            if (currentFlyHeight >= flyHeight)
            { 
                phase = 3;
                currentFlyHeight = flyHeight;
            }
        }

        Vector3 position = transform.position;
        position.y = currentFlyHeight;  // Фиксированная глобальная высота
        transform.position = position;
    }

    void UpdateDestination() 
    {
        if (patrolPoints.Count == 0) return;
        currentPoint++;
        if (currentPoint == patrolPoints.Count) currentPoint = 0;

        destination = patrolPoints[currentPoint].position;
    }

    public void Launch()
    {
        phase = 2;
    }
}
