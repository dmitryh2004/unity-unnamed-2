using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlightController : MonoBehaviour
{
    public GameObject flashlightLight;

    private bool _active = false;
    private bool flashlightButtonPressed;
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
        controls.Gameplay.Flashlight.performed += UpdateFlashlight;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void UpdateFlashlight(InputAction.CallbackContext context)
    {
        _active = !_active;
        flashlightLight.SetActive(_active);
    }
}
