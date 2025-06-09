using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryController : MonoBehaviour
{
    private PlayerControls controls;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas inventoryUI;
    [SerializeField] InventoryUIController inventoryUIController;
    Animator inventoryUIAnimator;
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

        inventoryUIAnimator = inventoryUI.GetComponent<Animator>();
        UpdateAnimator();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OpenInventory();
    }

    public void CloseInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("Esc pressed");
        if (playerInput.currentActionMap.name == "InventoryUI")
            CloseInventory();
    }

    void OpenInventory()
    {
        Debug.Log("ToggleInventory called");
        visible = !visible;

        UpdateCurrentInputMap();

        UpdateAnimator();
        Debug.Log($"Current inventory: {InventorySystem.Instance.GetInventoryDataJson()}");
    }

    public void CloseInventory()
    {
        visible = false;

        UpdateCurrentInputMap();

        UpdateAnimator();
    }

    void UpdateCurrentInputMap()
    {
        if (visible)
        {
            InputActionMapSwitcher.Instance.SwitchMap("InventoryUI");
            inventoryUIController.UpdateInventory();
        }
        else
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
        }
    }

    void UpdateAnimator()
    {
        inventoryUIAnimator.SetBool("visible", visible);
    }

    public bool IsInventoryVisible()
    {
        return visible;
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        inventoryUIController.DropActiveItem(transform);
    }
}
