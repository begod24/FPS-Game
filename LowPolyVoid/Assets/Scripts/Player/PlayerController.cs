using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask = 1;
    [SerializeField] private Transform groundCheck;
    #endregion

    #region Private Fields
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private float xRotation;
    
    // Input values
    private Vector2 movementInput;
    private Vector2 lookInput;
    private bool isJumping;
    private bool isSprinting;
    private bool isCrouching;
    #endregion
    #region Unity Lifecycle
    private void Start()
    {
        InitializeComponents();
        SetupCursor();
        SubscribeToInputEvents();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleLook();
        HandleGroundCheck();
    }

    private void OnDestroy()
    {
        UnsubscribeFromInputEvents();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void SetupCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SubscribeToInputEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpPressed += OnJumpPressed;
            InputManager.Instance.OnSprintPressed += OnSprintPressed;
            InputManager.Instance.OnSprintReleased += OnSprintReleased;
            InputManager.Instance.OnCrouchPressed += OnCrouchPressed;
            InputManager.Instance.OnCrouchReleased += OnCrouchReleased;
            InputManager.Instance.OnAttackPressed += OnAttackPressed;
            InputManager.Instance.OnInteractPressed += OnInteractPressed;
        }
    }

    private void UnsubscribeFromInputEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpPressed -= OnJumpPressed;
            InputManager.Instance.OnSprintPressed -= OnSprintPressed;
            InputManager.Instance.OnSprintReleased -= OnSprintReleased;
            InputManager.Instance.OnCrouchPressed -= OnCrouchPressed;
            InputManager.Instance.OnCrouchReleased -= OnCrouchReleased;
            InputManager.Instance.OnAttackPressed -= OnAttackPressed;
            InputManager.Instance.OnInteractPressed -= OnInteractPressed;
        }
    }
    #endregion
    #region Input Handling
    private void HandleInput()
    {
        if (InputManager.Instance == null) return;
        
        movementInput = InputManager.Instance.MovementInput;
        lookInput = InputManager.Instance.LookInput;
        isJumping = InputManager.Instance.JumpPressed;
        isSprinting = InputManager.Instance.SprintPressed;
        isCrouching = InputManager.Instance.CrouchPressed;
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        CheckGroundStatus();
        ProcessMovement();
        ProcessJumping();
        ApplyGravity();
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
    }

    private void ProcessMovement()
    {
        Vector3 direction = transform.right * movementInput.x + transform.forward * movementInput.y;
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        characterController.Move(direction * currentSpeed * Time.deltaTime);
    }

    private void ProcessJumping()
    {
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
    }
    #endregion

    #region Look Controls
    private void HandleLook()
    {
        if (cameraTransform == null) return;
        
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    #endregion
    #region Input Event Handlers
    private void OnJumpPressed()
    {
        // Jump logic is handled in HandleMovement()
        Debug.Log("Jump pressed!");
    }

    private void OnSprintPressed() => Debug.Log("Sprint started!");
    private void OnSprintReleased() => Debug.Log("Sprint ended!");

    private void OnCrouchPressed()
    {
        Debug.Log("Crouch pressed!");
        // TODO: Implement crouch logic
    }

    private void OnCrouchReleased()
    {
        Debug.Log("Crouch released!");
        // TODO: Implement crouch release logic
    }

    private void OnAttackPressed()
    {
        Debug.Log("Attack pressed!");
        // TODO: Implement attack logic
    }

    private void OnInteractPressed()
    {
        Debug.Log("Interact pressed!");
        // TODO: Implement interaction logic
    }
    #endregion

    #region Debug Visualization
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        }
    }
    #endregion
}
