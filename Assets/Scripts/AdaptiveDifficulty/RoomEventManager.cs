using UnityEngine;

public class RoomEventManager : MonoBehaviour
{
    [SerializeField] RoomObject roomObject = null;

    [Header("Activity diffs")]
    [SerializeField] float firstEnterActivity = .5f;
    [SerializeField] float firstDoorOpenedActivity = 1f;
    [SerializeField] float nextEntersActivity = .1f;
    [SerializeField] float hackAttemptActivity = 1f;
    [SerializeField] float alarmRaisedActivity = 5f;
    [SerializeField] float safeOpenedActivity = .5f;
    [SerializeField] float tableOpenedActivity = .25f;
    [SerializeField] float lootPickedUpActivity = .05f;

    [Header("Variables")]
    [SerializeField] float nextEnterDelay = 60f;

    // variables
    bool firstEnter = true;
    float nextEnterTimer = 0f;

    private void Start()
    {
        if (roomObject == null) roomObject = GetComponent<RoomObject>();
    }
    public void FirstDoorOpenedEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            roomObject.ModifyActivity(firstDoorOpenedActivity);
    }
    public void RoomEnterEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
        {
            if (!firstEnter)
            {
                if (nextEnterTimer > nextEnterDelay)
                {
                    roomObject.ModifyActivity(nextEntersActivity);
                    nextEnterTimer = 0f;
                }
            }
            else
            {
                firstEnter = false;
                roomObject.ModifyActivity(firstEnterActivity);
            }
        }
    }
    public void HackAttemptEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            roomObject.ModifyActivity(hackAttemptActivity);
    }
    public void AlarmRaisedEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            roomObject.ModifyActivity(alarmRaisedActivity);
    }
    public void SafeOpenedEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            roomObject.ModifyActivity(safeOpenedActivity);
    }
    public void TableOpenedEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            roomObject.ModifyActivity(tableOpenedActivity);
    }
    public void LootPickedUpEvent()
    {
        if (AdaptiveDifficultyManager.Instance.UseRoomWeights)
            roomObject.ModifyActivity(lootPickedUpActivity);
    }

    private void Update()
    {
        if (!firstEnter)
            nextEnterTimer += Time.deltaTime;
    }
}
