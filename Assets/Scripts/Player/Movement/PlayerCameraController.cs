using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform; // ������ �� Transform ������ (��� � ��������)
    public float mouseSensitivity = 2f;
    public float cameraPitchMin = -70f;
    public float cameraPitchMax = 70f;

    private float cameraPitch = 0f; // ������� ������ ������ �� X

    private PlayerControls controls;
    private Vector2 lookInput;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Look.canceled += _ => lookInput = Vector2.zero;
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // �������� ��������� �� ��� Y (�����������)
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0, mouseX, 0);

        // �������� ������ �� ��� X (���������)
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, cameraPitchMin, cameraPitchMax);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
    }
}
