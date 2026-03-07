using UnityEngine;

public class AchievementEraseButton : HoldButtonController
{
    [SerializeField] AchievementUIController uiController;
    protected override void OnHoldComplete()
    {
        AchievementActionTracker.Instance.EraseAchievementData();
        uiController.UpdateUI();
    }
}
