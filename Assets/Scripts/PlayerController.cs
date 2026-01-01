using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using FMODUnity;
using FMOD.Studio;
using System.Collections; // Needed for coroutines

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

    [Header("FMOD Events")]
    [SerializeField] private EventReference bombSound;
    [SerializeField] private EventReference bombSnapshot;

    [Header("Prefabs")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Transform canvasTransform; // drag your Canvas from Hierarchy here

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

        // E key check (new Input System)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerBomb();
        }
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

    // Bomb trigger logic
    private void TriggerBomb()
    {
        // Play bomb sound
        RuntimeManager.PlayOneShot(bombSound);

        // Start snapshot (optional)
        EventInstance snapshot = RuntimeManager.CreateInstance(bombSnapshot);
        snapshot.start();
        snapshot.release();

        // Find Canvas in scene if not assigned
        if (canvasTransform == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null) canvasTransform = canvas.transform;
        }

        // Show speech bubble with bounce animation
        if (speechBubblePrefab != null && canvasTransform != null)
        {
            GameObject bubble = Instantiate(speechBubblePrefab, canvasTransform);
            bubble.transform.localScale = Vector3.zero; // start invisible
            bubble.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // center of screen
            StartCoroutine(AnimateBubble(bubble));
        }
    }

    // Coroutine for bounce overshoot zoom in/out animation
    private IEnumerator AnimateBubble(GameObject bubble)
    {
        // Zoom in with overshoot
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 4; // speed factor
            float scale = Mathf.Lerp(0f, 1.2f, t); // overshoot to 1.2
            bubble.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        // Ease back to normal size
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 4;
            float scale = Mathf.Lerp(1.2f, 1f, t);
            bubble.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        // Wait before disappearing
        yield return new WaitForSeconds(1.5f);

        // Zoom out
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 4;
            float scale = Mathf.Lerp(1f, 0f, t);
            bubble.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        Destroy(bubble);
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