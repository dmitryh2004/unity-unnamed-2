using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable] public enum FaceDirection
{
    forward = 0, back = 1, right = 2, left = 3, up = 4, down = 5
}

public class GuardianController : MonoBehaviour
{
    System.Random random = new ();

    int phase = 1;
    float phaseUpdateTimer = 0f;
    float findInteractablesTimer = 0f;
    bool active = false;
    [SerializeField] bool isActiveOnStart = true;

    //Phase 1
    bool goForward = true;
    int currentPoint = 0;
    public List<Interactable> checkedInteractables = new();
    Dictionary<int, bool> checkedDoorsStates = new();
    Dictionary<int, bool> checkedLocksStates = new();
    Dictionary<int, int> checkedLocksDifficulties = new();

    //Phase 2
    Transform target = null;

    //Phase 3
    float phase3Timer = 0f;
    float phase3TurnAroundTimer = 0f;
    float currentRotationAngle = 0f;

    //Footsteps
    float footstepTimer = 0f;
    bool footstepTimerActive = true;

    //Animator
    float animatorPhase1MovingSpeed = 0f, animatorPhase2MovingSpeed = 0f;
    Material instanceMaterial;

    //Stuck management
    float stuckTimer = 0f;
    [SerializeField] float stuckDetectTime = 5f;
    [SerializeField] float stuckVelocity = .5f;

    //Adaptive Difficulty
    float AD_movingSpeedMultiplier = 1f;
    float AD_xrayRangeMultiplier = 1f;
    float AD_sightRangeMultiplier = 1f;
    float AD_phase3DurationMultiplier = 1f;
    bool AD_checkDoors = false;
    bool AD_checkFloorItems = false;
    bool AD_checkLockActivity = false;
    bool AD_checkLockDifficulty = false;
	
	// Misc
	bool hasPineapple = false;

    [Header("Links")]
    [SerializeField] Transform headObj;
    [SerializeField] Light fovLight;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GuardianAudioPlayer audioPlayer;
    [SerializeField] List<Transform> possibleTargetObjects = new ();
    [SerializeField] GameObject patrolPointPrefab;
    [SerializeField] Transform levelPatrolPoints;
    [SerializeField] Animator animator;
    [SerializeField] Renderer headRenderer;
	[SerializeField] GameObject pineappleObject;

    [Header("Common Settings")]
    [SerializeField] FaceDirection headFaceDirection;
    [SerializeField] LayerMask pointVisibilityCheckMask;
    [SerializeField] float speed = 2f;
    [SerializeField] float runningSpeed = 6f;
    [SerializeField] float xraySpotDistance = 1f;
    [SerializeField] float maxSpotDistance = 10f;
    [SerializeField] float fov = 60f;
    [SerializeField] float phaseUpdateInterval = .5f;
    [SerializeField] float findInteractablesInterval = 5f;
    [SerializeField] bool addDestinationsToPatrolPoints = false;
    [SerializeField] float destStopDistance = .5f, attackDistance = 1f;

    [Header("Phase 1 Settings")]
    [SerializeField] List<Transform> patrolPoints = new ();
    [SerializeField] bool enterPhase3OnPoints = true;

    [Header("Phase 2 Settings")]
    [SerializeField] bool openClosedDoors = true;
    [SerializeField] float phase2Fov = 90f;

    [Header("Phase 3 Settings")]
    [SerializeField] float delay = 7.5f;
    [SerializeField] float turnAroundInterval = 2.5f;
    [SerializeField] float baseRotationAngle = 180f;

    [Header("Phase Emission Colors")]
    [SerializeField] Color phase1EmissionColor = Color.blue;
    [SerializeField] Color phase2EmissionColor = Color.red;
    [SerializeField] Color phase3EmissionColor = Color.yellow;
	
	[Header("Misc")]
	[SerializeField] float pineappleSpawnChance = 0.1f;

    public bool CanOpenClosedDoors() => openClosedDoors;
    bool RunAdaptiveDifficultyFunctional() => AD_checkDoors || AD_checkFloorItems || AD_checkLockActivity || AD_checkLockDifficulty;

    public Light FovLight => fovLight;

    private void Awake()
    {
        instanceMaterial = headRenderer.material;
    }

    public void SetActive(bool active)
    {
        this.active = active;

        if (active)
        {
            SwitchPhase(1);
            PickFirstWaypoint();
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("moving", false);
                animator.SetFloat("moveSpeed", 1f);
            }
        }
    }

    public bool IsPointVisible(Transform point)
    {
        Vector3 direction = point.position - headObj.position;
        //Debug.Log($"dir: {point.position} - {headObj.position} = {direction}");
        if (direction.magnitude < xraySpotDistance * AD_xrayRangeMultiplier) return true;

        if (direction.magnitude > maxSpotDistance * AD_sightRangeMultiplier) return false;

        Vector3 facingDirection = GetHeadFacingDirection();
        //Debug.Log($"face: {facingDirection}");
        float dot = Vector3.Dot(facingDirection.normalized, direction.normalized);

        float minDot = Mathf.Deg2Rad * GetCurrentFov();
        //Debug.Log($"dot: {dot}");
        if (dot < Mathf.Cos(minDot)) return false;
        
        RaycastHit hit;
        if (Physics.Raycast(headObj.position, direction, out hit, maxSpotDistance * AD_sightRangeMultiplier, pointVisibilityCheckMask.value, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log($"{hit.collider.gameObject.name} (pos: {hit.collider.transform.position})");
            if (hit.collider.transform == point) return true;
        }
        return false;
    }

    void FindInteractables()
    {
        //Debug.Log($"Searching for interactables... (currently found: {checkedInteractables.Count})");
        List<Collider> possibleTargets = Physics.OverlapSphere(transform.position, maxSpotDistance * AD_sightRangeMultiplier, 64).ToList();
        foreach (Collider col in possibleTargets)
        {
            Interactable interactable;
            if (col.TryGetComponent(out interactable))
            {
                if (checkedInteractables.Contains(interactable)) continue;
                checkedInteractables.Add(interactable);
            }
        }
    }

    Vector3 GetHeadFacingDirection()
    {
        switch (headFaceDirection)
        {
            case FaceDirection.forward:
                return headObj.forward;
            case FaceDirection.back:
                return -headObj.forward;
            case FaceDirection.right:
                return headObj.right;
            case FaceDirection.left:
                return -headObj.right;
            case FaceDirection.up:
                return headObj.up;
            case FaceDirection.down:
                return -headObj.up;
            default:
                return headObj.forward;
        }
    }

    Color GetPhaseEmissionColor()
    {
        if (!active) return Color.black;
        switch (phase)
        {
            case 1:
                return phase1EmissionColor;
            case 2:
                return phase2EmissionColor;
            case 3:
                return phase3EmissionColor;
            default:
                return Color.black;
        }
    }

    private void AdjustToAdaptiveDifficulty()
    {
        if (AdaptiveDifficultyManager.Instance != null)
        {
            int alertnessDegree = AdaptiveDifficultyManager.Instance.AlertnessDegree();

            AD_movingSpeedMultiplier = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianSpeedMultiplier", alertnessDegree) ?? AD_movingSpeedMultiplier;
            AD_sightRangeMultiplier = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianSightRangeMultiplier", alertnessDegree) ?? AD_sightRangeMultiplier;
            AD_xrayRangeMultiplier = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianXrayRangeMultiplier", alertnessDegree) ?? AD_xrayRangeMultiplier;
            AD_phase3DurationMultiplier = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianCheckPointTimeMultiplier", alertnessDegree) ?? AD_phase3DurationMultiplier;
            AD_checkDoors = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianCheckDoors", alertnessDegree) > 0;
            AD_checkFloorItems = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianCheckFloorItems", alertnessDegree) > 0;
            AD_checkLockActivity = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianCheckLockActivity", alertnessDegree) > 0;
            AD_checkLockDifficulty = AdaptiveDifficultyManager.Instance.Values.GetParameterValue("GuardianCheckLockDifficulty", alertnessDegree) > 0;
            //Debug.Log($"Adaptive difficulty\nCheck doors: {AD_checkDoors}, Check floor items: {AD_checkFloorItems}, Check lock activity: {AD_checkLockActivity}, Check lock difficulty: {AD_checkLockDifficulty}");
        }
    }

    private void Start()
    {
		float randomValue = (float)(random.NextDouble());
		hasPineapple = randomValue < pineappleSpawnChance;
		pineappleObject.SetActive(hasPineapple);
		
        AdjustToAdaptiveDifficulty();

        animatorPhase1MovingSpeed = AD_movingSpeedMultiplier;
        animatorPhase2MovingSpeed = runningSpeed / speed * AD_movingSpeedMultiplier;

        currentRotationAngle = baseRotationAngle;
    }

    public void Init()
    {
        UpdateFovLight();

        SetActive(isActiveOnStart);
    }

    float GetCurrentFov()
    {
        return (phase != 2) ? fov : phase2Fov;
    }

    void UpdateFovLight()
    {
        fovLight.range = maxSpotDistance * AD_sightRangeMultiplier;
        float angle = GetCurrentFov();
        fovLight.spotAngle = angle;
        fovLight.innerSpotAngle = angle;
    } 

    public void SwitchPhase(int newPhase, bool raiseAlarm = false)
    {
        //Debug.Log($"{gameObject.name}: switching to phase {newPhase}");
        phase = newPhase; //change phase
        UpdateFovLight();
        StopAllCoroutines();
        StartCoroutine(SmoothlyRotate(baseRotationAngle - currentRotationAngle, playSounds: false));

        instanceMaterial.SetColor("_EmissionColor", GetPhaseEmissionColor());
        switch (phase) //update npc
        {
            case 1:
                agent.speed = speed * AD_movingSpeedMultiplier;
                if (animator != null)
                {
                    animator.SetBool("moving", true);
                    animator.SetFloat("moveSpeed", animatorPhase1MovingSpeed);
                }

                footstepTimerActive = true;
                SetNextWaypoint();
                break;
            case 2:
                agent.speed = runningSpeed * AD_movingSpeedMultiplier;
                if (animator != null)
                {
                    animator.SetBool("moving", true);
                    animator.SetFloat("moveSpeed", animatorPhase2MovingSpeed);
                }

                footstepTimerActive = true;
                if (raiseAlarm && !AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StartAlarm();
                agent.SetDestination(target.position);
                break;
            case 3:
                agent.speed = speed * AD_movingSpeedMultiplier;
                agent.destination = transform.position;

                if (animator != null)
                {
                    animator.SetBool("moving", false);
                    animator.SetFloat("moveSpeed", animatorPhase1MovingSpeed);
                }
                footstepTimerActive = false;
                phase3Timer = 0f;
                phase3TurnAroundTimer = 0f;
                break;
        }
    }

    private void Update()
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = !active;
        if (!active) return;
        // find interactables
        if (RunAdaptiveDifficultyFunctional())
        {
            findInteractablesTimer += Time.deltaTime;
            if (findInteractablesTimer > findInteractablesInterval)
            {
                FindInteractables();
                findInteractablesTimer = 0f;
            }
        }

        // footsteps
        if (footstepTimerActive)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer > 2f / agent.speed)
            {
                audioPlayer.PlayFootstepAudio();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        // phase update
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

        // stuck check
        CheckForStuck();
    }

    public void CheckForStuck()
    {
        bool isMoving = agent.velocity.magnitude > stuckVelocity * AD_movingSpeedMultiplier;
        if (phase == 3 || isMoving)
        {
            stuckTimer = 0f;
        }
        else
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckDetectTime)
            {
                stuckTimer = 0f;
                if (phase == 1)
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
                else if (phase == 2)
                {
                    SwitchPhase(3);
                }
            }
        }
    }

    private void SetNextWaypoint()
    {
        currentPoint += ((goForward) ? 1 : -1);
        if (currentPoint == 0 || currentPoint == patrolPoints.Count - 1) goForward = !goForward;

        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    private void PickFirstWaypoint()
    {
        agent.SetDestination(patrolPoints[0].position);
        currentPoint = 0;
        goForward = true;
    }

    private void CheckForTrackedObjects()
    {
        // check for tracked objects
        foreach (var possibleTarget in possibleTargetObjects)
        {
            if (IsPointVisible(possibleTarget))
            {
                target = possibleTarget;
                SwitchPhase(2, true);
                break;
            }
        }

        // adaptive difficulty check
        if (RunAdaptiveDifficultyFunctional())
        {
            for (int i = 0; i < checkedInteractables.Count; i++)
            {
                Interactable interactable = checkedInteractables[i];
                if (interactable == null) continue;
                if (!IsPointVisible(interactable.transform)) // if guardian can not see the interactable, skip it
                {
                    //Debug.Log($"interactable #{i} is not visible - skip (pos: {interactable.transform.position})");
                    continue;
                }

                if (AD_checkDoors && interactable is DoorController door) // door
                {
                    //Debug.Log($"interactable #{i} is a door");
                    if (checkedDoorsStates.ContainsKey(i)) // door was checked earlier
                    {
                        bool lastDoorState = checkedDoorsStates[i];
                        bool currentDoorState = door.IsOpen();
                        //Debug.Log($"door #{i} last state: {lastDoorState}, current state: {currentDoorState}");

                        if (lastDoorState != currentDoorState) // if door states don't equal, raise alarm
                        {
                            if (!AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StartAlarm();
                        }
                    }
                    else // door was seen first time
                    {
                        bool doorState = door.IsOpen();
                        //Debug.Log($"add door #{i} to the dict (state: {doorState})");
                        checkedDoorsStates[i] = doorState;
                    }
                }
                else if ((AD_checkLockActivity || AD_checkLockDifficulty) && interactable is LockController _lock) // lock
                {
                    if (AD_checkLockActivity) // check lock activity
                    {
                        if (checkedLocksStates.ContainsKey(i)) // lock was checked earlier
                        {
                            bool lastLockActivity = checkedLocksStates[i];
                            bool currentLockActivity = _lock.IsActive();

                            if (lastLockActivity != currentLockActivity) // if lock states don't equal, raise alarm
                            {
                                if (!AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StartAlarm();
                            }
                        }
                        else // lock was seen first time
                        {
                            bool lockActivity = _lock.IsActive();
                            checkedLocksStates[i] = lockActivity;
                        }
                    }
                    if (AD_checkLockDifficulty) // check lock difficulty
                    {
                        if (checkedLocksDifficulties.ContainsKey(i)) // lock was checked earlier
                        {
                            int lastLockDifficulty = checkedLocksDifficulties[i];
                            int currentLockDifficulty = _lock.GetDifficulty();

                            if (lastLockDifficulty != currentLockDifficulty) // if lock difficulties don't equal, raise alarm
                            {
                                if (!AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StartAlarm();
                            }
                        }
                        else // lock was seen first time
                        {
                            int lockDifficulty = _lock.GetDifficulty();
                            checkedLocksDifficulties[i] = lockDifficulty;
                        }
                    }
                }
                else if (AD_checkFloorItems && interactable is LootableItem lootableItem && !lootableItem.GetComponentInParent<LootContainer>()) // весь лут спавнится как дочерний для LootContainer,
                    // а брошенный игроком лут спавнится в корне сцены и не имеет родительского LootContainer
                {
                    if (!AlarmController.Instance.GetAlarmState()) AlarmController.Instance.StartAlarm();
                }
            }
        }
    }

    private void Phase1Update()
    {
        //Debug.Log($"{gameObject.name}: phase 1 update");
        // update npc movement
        if (agent.destination != null)
        {
            if (agent.remainingDistance < destStopDistance)
            {
                //Debug.Log($"{gameObject.name}: dest={agent.destination}, dist={agent.remainingDistance}, pos={transform.position}");
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
            //Debug.Log($"{gameObject.name}: dest=null");
            PickFirstWaypoint();
        }

        CheckForTrackedObjects();
    }

    private void Phase2Update()
    {
        // check for player in range 1m
        if (target != null)
        {
            if (Vector3.Distance(target.position, transform.position) < attackDistance)
            {
                SetActive(false);
				
				if (hasPineapple)
					audioPlayer.PlayPineappleAttackAudio();
				else
					audioPlayer.PlayAttackAudio();
                
				if (animator != null)
                    animator.SetTrigger("attack");

                LevelManager.Instance.GameOver(1);
            }
        }
        
        // update npc movement
        if (agent.destination != null)
        {
            if (agent.remainingDistance < destStopDistance) // if npc is near the dest point, switch phase to 3
            {
                if (addDestinationsToPatrolPoints)
                {
                    //check for absence of patrol points in 2 meters around
                    Vector3 destination = agent.destination;
                    bool noPatrolPoints = true;
                    foreach (Transform pp in patrolPoints)
                    {
                        if (Vector3.Distance(pp.position, destination) < 2f)
                        {
                            noPatrolPoints = false;
                            break;
                        }
                    }

                    if (noPatrolPoints)
                    {
                        GameObject newPatrolPoint = Instantiate(patrolPointPrefab, destination, Quaternion.Euler(0, 0, 0), levelPatrolPoints);
                        patrolPoints.Add(newPatrolPoint.transform);
                    }
                }
                
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
        if (phase3Timer >= delay * AD_phase3DurationMultiplier) // exit to phase 1
        {
            SwitchPhase(1);
            return;
        }

        // handle turnaround
        if (phase3TurnAroundTimer >= turnAroundInterval)
        {
            phase3TurnAroundTimer = 0f;
            float randomAngle = 4 * fov * (float)(random.NextDouble() - 0.5);
            //print($"{name}: rotate head by {randomAngle}");
            StartCoroutine(SmoothlyRotate(randomAngle));
        }

        CheckForTrackedObjects();
    }

    IEnumerator SmoothlyRotate(float angle, bool playSounds = true)
    {
        float rotated = 0f;
        if (playSounds)
            audioPlayer.PlayRotateAudio();
        while (Mathf.Abs(rotated) < Mathf.Abs(angle))
        {
            float frameRotation = Mathf.Sign(angle) * agent.angularSpeed * Time.deltaTime;
            headObj.Rotate(new Vector3(0f, frameRotation, 0f));
            rotated += frameRotation;
            currentRotationAngle += frameRotation;
            //print($"{name}: rotated {Mathf.Abs(rotated)}/{Mathf.Abs(angle)}");
            yield return null;
        }
        if (playSounds)
            audioPlayer.PlayRotateAudio();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetPossibleTargetObjects(List<Transform> possibleTargetObjects) => this.possibleTargetObjects = possibleTargetObjects;
    public void SetPatrolPoints(List<Transform> patrolPoints) => this.patrolPoints = patrolPoints;
    public void SetAddDestinationsToPatrolPoints(bool value) => addDestinationsToPatrolPoints = value;
    public void SetEnterPhase3OnPoints(bool value) => enterPhase3OnPoints = value;

    public void CallGuardian(Transform target, Vector3 position)
    {
        SetTarget(target);
        SwitchPhase(2);
        agent.SetDestination(position);
    }

    public void UpdateInteractableState(Interactable interactable, bool newState)
    {
        for (int i = 0; i < checkedInteractables.Count; i++)
        {
            if (checkedInteractables[i] == interactable)
            {
                if (interactable is DoorController)
                {
                    checkedDoorsStates[i] = newState;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(headObj.position, xraySpotDistance * AD_xrayRangeMultiplier);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(headObj.position, GetHeadFacingDirection() * maxSpotDistance * AD_sightRangeMultiplier);
    }
}
