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

    LockController subject;

    [Header("Active element hint")]
    [Tooltip("Animator")]
    [SerializeField] Animator activeElementHintAnimator;

    [Tooltip("Title")]
    [SerializeField] TMP_Text activeElementHintTitle;
    [Tooltip("Text")]
    [SerializeField] TMP_Text activeElementHintText;

    Node hoveredNode = null;

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
                int value1 = node.GetValue1();
                aehText = nodeType.nodeDesc.Replace("X", $"{value1}");
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

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (hoveredNode == null) return;

        hoveredNode.Interact();

        gridController.MakeStep();
    }

    public GridController GetGridController()
    {
        return gridController;
    }
}
