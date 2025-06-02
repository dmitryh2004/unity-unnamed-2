using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 8f;
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float heightChangeSpeed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheckPoint;

    private float standScale = .9f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isCrouching;
    private bool isSprinting;
    private float currentHeight;
    private float currentSpeed;
    private bool isJumpPressed;
    private bool isGrounded;

    private PlayerControls controls;
    PlayerInventoryController inventoryController;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inventoryController = GetComponent<PlayerInventoryController>();
        controls = new PlayerControls();
        currentHeight = standHeight;
        standScale = transform.localScale.y;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public void UpdateMove(InputAction.CallbackContext context)
    {
        if (context.started) return;

        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"{gameObject.name}: move input={moveInput}");
    }


    public void UpdateCrouch(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        isCrouching = value > 0;
    }

    public void UpdateSprint(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        isSprinting = value > 0 && !isCrouching;
    }

    public void UpdateJump(InputAction.CallbackContext context)
    {
        isJumpPressed = (context.performed) ? true : false;
    }

    void Update()
    {
        // Определяем скорость
        currentSpeed = moveSpeed;
        if (isSprinting) currentSpeed = sprintSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;

        // Изменяем высоту персонажа (например, через коллайдер или визуально)
        currentHeight = Mathf.Lerp(currentHeight, isCrouching ? crouchHeight : standHeight, Time.deltaTime * heightChangeSpeed);
        transform.localScale = new Vector3(transform.localScale.x, currentHeight / standHeight * standScale, transform.localScale.z);

        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        rb.linearVelocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);

        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
            isJumpPressed = false; // Сброс, чтобы не прыгал повторно в этом кадре
        }
    }
}
