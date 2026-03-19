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
    [SerializeField] PlayerAudioPlayer audioPlayer;
    private float footstepTimer = 0f;

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

    private void Update()
    {
        UpdateSpeed();
        UpdateFootstepTimer();
    }

    void UpdateFootstepTimer()
    {
        if (moveInput != Vector2.zero && isGrounded && !isCrouching)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= 2f / currentSpeed)
            {
                footstepTimer = 0f;
                audioPlayer.PlayFootstepAudio();
            }
        }
        else
            footstepTimer = 0f;
    }

    void UpdateSpeed()
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
        // базовое желаемое направление (в локальных осях)
        Vector3 inputDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        // по умолчанию двигаемся просто по XZ
        Vector3 moveDirection = inputDir;

        // если на земле — проецируем направление на плоскость склона
        if (isGrounded)
        {
            // получаем нормаль поверхности под ногами
            if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out RaycastHit hit, 1f, groundMask))
            {
                Vector3 groundNormal = hit.normal;

                // проекция направления на плоскость с этой нормалью
                moveDirection = Vector3.ProjectOnPlane(inputDir, groundNormal).normalized;
            }
        }

        // задаём скорость вдоль плоскости
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;
        rb.linearVelocity = velocity;

        // прыжок
        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
            isJumpPressed = false;
        }
    }

}
