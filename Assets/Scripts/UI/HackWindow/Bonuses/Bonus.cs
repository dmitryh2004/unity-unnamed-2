using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Bonus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected System.Random random = new();

    protected BonusType bonusType;
    protected bool active = false;
    [SerializeField] protected Animator animator;

    protected bool useDuration = true;
    protected int duration = 0;
    [SerializeField] protected TMP_Text durationText;

    protected bool useTarget = false;
    protected Node target = null;

    protected int value1, value2, value3, value4;

    public abstract void Initialize();
    public abstract void MakeStep();

    public void StartTargeting()
    {
        animator.SetBool("target", true);
    }

    public void StopTargeting()
    {
        animator.SetBool("target", false);
    }

    public abstract void Use();

    public void SetTarget(Node node) 
    {
        target = node;
    }

    public void SetBonusType(BonusType bonusType)
    {
        this.bonusType = bonusType;
    }

    public BonusType GetBonusType() => bonusType;
    public bool IsActive() => active;
    public Animator GetAnimator() => animator;
    public bool UseDuration() => useDuration;
    public int GetDuration() => duration;
    public bool UseTarget() => useTarget;
    public Node GetTarget() => target;
    public int GetValue1() => value1;
    public int GetValue2() => value2;
    public int GetValue3() => value3;
    public int GetValue4() => value4;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsActive())
            HackWindowController.Instance.SetHoveredBonus(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HackWindowController.Instance.ClearHoveredBonus();
    }
}
