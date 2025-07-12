using UnityEngine;
using UnityEngine.InputSystem;

public class ChestController : Interactable
{
    private PlayerControls controls;
    [SerializeField] PlayerInput playerInput;
    [Header("Inventory")]
    [SerializeField] Canvas chestUI;
    [SerializeField] ChestUIController chestUIController;
    Animator chestUIAnimator;
    private bool visible = false;

    private void Awake()
    {
        controls = new PlayerControls();
    }
    void OnEnable()
    {
        controls.Enable();
        //InventoryAction.performed += ToggleInventory;
        //CloseInventoryAction.performed += ToggleInventory;

        chestUIAnimator = chestUI.GetComponent<Animator>();
        UpdateAnimator();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public void OpenChest(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OpenChest();
    }

    public void CloseChest(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("Esc pressed");
        if (playerInput.currentActionMap.name == "ChestUI")
            CloseChest();
    }

    public void ScrollUpInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "ChestUI") ScrollUpInventory();
    }

    public void ScrollUpInventory()
    {
        chestUIController.ScrollUp();
    }

    public void ScrollDownInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "ChestUI") ScrollDownInventory();
    }

    public void ScrollDownInventory()
    {
        chestUIController.ScrollDown();
    }

    public void ScrollUpChest(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "ChestUI") ScrollUpChest();
    }

    public void ScrollUpChest()
    {
        chestUIController.ChestScrollUp();
    }

    public void ScrollDownChest(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "ChestUI") ScrollDownChest();
    }

    public void ScrollDownChest()
    {
        chestUIController.ChestScrollDown();
    }

    void OpenChest()
    {
        visible = !visible;

        UpdateCurrentInputMap();

        chestUIController.ClearOffset();
        chestUIController.ClearChestOffset();
        UpdateAnimator();
    }

    public void CloseChest()
    {
        visible = false;

        UpdateCurrentInputMap();

        UpdateAnimator();
    }

    void UpdateCurrentInputMap()
    {
        if (visible)
        {
            InputActionMapSwitcher.Instance.SwitchMap("ChestUI");
            chestUIController.UpdateInventory();
            chestUIController.UpdateChest();
        }
        else
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
        }
    }

    void UpdateAnimator()
    {
        chestUIAnimator.SetBool("visible", visible);
    }

    public bool IsChestUIVisible()
    {
        return visible;
    }

    public void TransferItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "ChestUI") chestUIController.TransferActiveItem();
    }

    public override void Interact()
    {
        OpenChest();
    }
}
