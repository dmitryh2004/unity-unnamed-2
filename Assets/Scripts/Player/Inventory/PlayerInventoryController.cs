using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryController : MonoBehaviour
{
    private PlayerControls controls;
    private InputAction showInventoryAction;

    private void Awake()
    {
        controls = new PlayerControls();
        showInventoryAction = controls.Gameplay.ShowInventory;
    }
    void OnEnable()
    {
        controls.Enable();
        showInventoryAction.performed += OnShowInventory;
    }

    void OnDisable()
    {
        controls.Disable();
    }

    private void OnShowInventory(InputAction.CallbackContext context)
    {
        Debug.Log($"Current inventory: {InventorySystem.Instance.GetInventoryDataJson()}");
    }
}
