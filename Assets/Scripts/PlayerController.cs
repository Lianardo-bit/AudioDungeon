using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Grounding Settings")]
    public float gravity = -20f;
    public float groundCheckRadius = 0.3f;
    public float stepOffset = 0.3f; // helps with uneven thresholds
    public LayerMask groundMask;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform; // The player's camera (child of the player)

    [Header("References")]
    public Transform groundCheck;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float currentSpeed;
    private bool isRunning;
    private bool isGrounded;
    private float xRotation = 0f; // pitch rotation for camera

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.stepOffset = stepOffset;

        // Lock cursor for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleGrounding();
        HandleMovement();
        HandleLook();
    }

    private void HandleGrounding()
    {
        // Sphere check at feet for ground detection
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // stick to ground without jitter
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // Input direction relative to player orientation (camera yaw)
        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDir.Normalize();

        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        float targetMagnitude = moveDir.magnitude * targetSpeed;

        // Smooth acceleration/deceleration
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetMagnitude,
            (targetMagnitude > currentSpeed ? acceleration : deceleration) * Time.deltaTime);

        Vector3 movement = moveDir * currentSpeed;
        controller.Move(movement * Time.deltaTime);
    }

    private void HandleLook()
    {
        // Mouse look input
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Pitch (camera up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (player body left/right)
        transform.Rotate(Vector3.up * mouseX);
    }

    // Input System Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
