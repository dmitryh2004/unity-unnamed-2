using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HackWindowController : MonoBehaviour
{
    private PlayerControls controls;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Animator hackUIAnimator;

    public static HackWindowController Instance = null;

    [SerializeField] string emptyVisitedNodeTitle, UnvisitedNodeTitle;
    [TextArea(6, 10)]
    [SerializeField] string emptyVisitedNodeText, UnvisitedNodeText;

    [SerializeField] GridController gridController;
    [SerializeField] BonusController bonusController;

    LockController subject;

    [Header("Active element hint")]
    [Tooltip("Animator")]
    [SerializeField] Animator activeElementHintAnimator;

    [Tooltip("Title")]
    [SerializeField] TMP_Text activeElementHintTitle;
    [Tooltip("Text")]
    [SerializeField] TMP_Text activeElementHintText;

    int interactMode = 0; // 0 - standart, 1 - select target
    Node hoveredNode = null;
    Bonus hoveredBonus = null;
    Bonus targetingBonus = null;

    int difficulty;
    private bool visible = false;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();

        hackUIAnimator = GetComponent<Animator>();
        UpdateAnimator();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void UpdateAnimator()
    {
        hackUIAnimator.SetBool("visible", visible);
    }

    public void OpenHackWindow(LockController lockController)
    {
        subject = lockController;
        difficulty = subject.GetDifficulty();
        gridController.Generate(difficulty);
        bonusController.ClearBonuses();

        VirusController.Instance.ResetToStart();
        visible = true;

        UpdateCurrentInputMap();

        UpdateAnimator();
    }

    public void CloseHackWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "HackUI")
        {
            FailLock();
        }
    }

    public void FailLock()
    {
        subject.IncreaseDifficulty(1);
        CloseHackWindow();
    }

    public void SuccessLock()
    {
        subject.DisableLock();
        CloseHackWindow();
    }

    public void CloseHackWindow()
    {
        visible = false;

        UpdateCurrentInputMap();

        UpdateAnimator();
    }

    void UpdateCurrentInputMap()
    {
        if (visible)
        {
            InputActionMapSwitcher.Instance.SwitchMap("HackUI");
        }
        else
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
        }
    }

    public void SetDifficulty(int diff)
    {
        difficulty = Mathf.Clamp(diff, 1, 10);
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetHoveredNode(Node node)
    {
        hoveredNode = node;
        string aehTitle = UnvisitedNodeTitle, aehText = UnvisitedNodeText;
        
        // visited inactive node
        if (node.IsVisited() && !node.IsActive())
        {
            aehTitle = emptyVisitedNodeTitle;
            aehText = emptyVisitedNodeText;
        }
        // active node
        else if (node.IsActive())
        {
            NodeType nodeType = hoveredNode.GetNodeType();

            if (nodeType != null)
            {
                aehTitle = nodeType.nodeName;
                if (node.IsBonus())
                {
                    int value1 = nodeType.bonus.value1ByDifficulty[difficulty - 1];
                    int value2 = nodeType.bonus.value2ByDifficulty[difficulty - 1];
                    int value3 = nodeType.bonus.value3ByDifficulty[difficulty - 1];
                    int value4 = nodeType.bonus.value4ByDifficulty[difficulty - 1];
                    aehText = nodeType.nodeDesc.Replace("X", $"{value1}")
                        .Replace("Y", $"{value2}")
                        .Replace("Z", $"{value3}")
                        .Replace("W", $"{value4}");
                }
                else
                {
                    int value1 = node.GetValue1();
                    aehText = nodeType.nodeDesc.Replace("X", $"{value1}");
                }
            }
        }

        activeElementHintTitle.text = aehTitle;
        activeElementHintText.text = aehText;
        activeElementHintAnimator.SetBool("visible", true);
    }

    public void ClearHoveredNode()
    {
        hoveredNode = null;
        activeElementHintAnimator.SetBool("visible", false);
    }

    public void SetHoveredBonus(Bonus bonus)
    {
        hoveredBonus = bonus;

        string aehTitle = bonus.GetBonusType().bonusName;
        string aehText = bonus.GetBonusType().bonusDesc;

        int value1 = bonus.GetValue1();
        int value2 = bonus.GetValue2();
        int value3 = bonus.GetValue3();
        int value4 = bonus.GetValue4();
        int duration = bonus.GetDuration();

        aehText = aehText.Replace("D", $"{duration}").Replace("X", $"{value1}").Replace("Y", $"{value2}").Replace("Z", $"{value3}").Replace("W", $"{value4}");

        activeElementHintTitle.text = aehTitle;
        activeElementHintText.text = aehText;
        activeElementHintAnimator.SetBool("visible", true);
    }
    public void ClearHoveredBonus()
    {
        hoveredBonus = null;
        activeElementHintAnimator.SetBool("visible", false);
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (hoveredNode == null && hoveredBonus == null) return;

        if (interactMode == 0)
        {
            bool correctTarget = false;

            if (hoveredBonus != null)
            {
                if (!hoveredBonus.IsActive())
                {
                    correctTarget = true;
                }
            }
            else
            {
                correctTarget = !(!hoveredNode.IsActive() && hoveredNode.IsVisited());
            }

            if (correctTarget)
            {
                gridController.MakeStepPre();
                if (hoveredNode != null)
                    hoveredNode.Interact();
                if (hoveredBonus != null)
                {
                    if (!hoveredBonus.UseTarget())
                    {
                        hoveredBonus.Use();
                    }
                    else
                    {
                        hoveredBonus.StartTargeting();
                        targetingBonus = hoveredBonus;
                        interactMode = 1;
                    }
                }
                gridController.MakeStepPost();
                bonusController.MakeStepPost();
            }
        }
        else
        {
            Debug.Log("Interact mode = 1");
            //checking that target is correct
            bool correctTarget = false;
            Debug.Log($"hovered node = {hoveredNode}");
            if (hoveredNode != null)
            {
                Debug.Log($"defensive: {hoveredNode.IsDefensiveNode()}; core: {hoveredNode.IsCoreNode()}; active: {hoveredNode.IsActive()}");
                if ((hoveredNode.IsDefensiveNode() || hoveredNode.IsCoreNode()) && hoveredNode.IsActive())
                {
                    correctTarget = true;
                    Debug.Log($"Setting correctTarget to true");
                }
            }

            targetingBonus.StopTargeting();
            if (correctTarget)
            {
                gridController.MakeStepPre();
                targetingBonus.SetTarget(hoveredNode);
                targetingBonus.Use();
                gridController.MakeStepPost();
                bonusController.MakeStepPost();
            }
            targetingBonus = null;
            interactMode = 0;
        }
    }

    public GridController GetGridController()
    {
        return gridController;
    }

    public BonusController GetBonusController()
    {
        return bonusController;
    }
}
