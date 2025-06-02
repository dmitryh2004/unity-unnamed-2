using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlightController : MonoBehaviour
{
    public GameObject flashlightLight;

    private bool _active = false;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        if (flashlightLight == null)
        {
            Debug.LogError($"{gameObject.name}: missing flashlightLight ref!");
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void UpdateFlashlight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _active = !_active;
        flashlightLight.SetActive(_active);
    }
}
