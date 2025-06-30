using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
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
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Subscribe to input events
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
    
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleLook();
        HandleGroundCheck();
    }
    
    private void HandleInput()
    {
        if (InputManager.Instance == null) return;
        
        movementInput = InputManager.Instance.MovementInput;
        lookInput = InputManager.Instance.LookInput;
        isJumping = InputManager.Instance.JumpPressed;
        isSprinting = InputManager.Instance.SprintPressed;
        isCrouching = InputManager.Instance.CrouchPressed;
    }
    
    private void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
        
        // Calculate movement direction
        Vector3 direction = transform.right * movementInput.x + transform.forward * movementInput.y;
        
        // Determine current speed
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        // Apply movement
        characterController.Move(direction * currentSpeed * Time.deltaTime);
        
        // Handle jumping
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    
    private void HandleLook()
    {
        if (cameraTransform == null) return;
        
        // Mouse look
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
    }
    
    #region Input Event Handlers
    
    private void OnJumpPressed()
    {
        Debug.Log("Jump pressed!");
        // Jump logic is handled in HandleMovement()
    }
    
    private void OnSprintPressed()
    {
        Debug.Log("Sprint started!");
    }
    
    private void OnSprintReleased()
    {
        Debug.Log("Sprint ended!");
    }
    
    private void OnCrouchPressed()
    {
        Debug.Log("Crouch pressed!");
        // Implement crouch logic here
    }
    
    private void OnCrouchReleased()
    {
        Debug.Log("Crouch released!");
        // Implement crouch release logic here
    }
    
    private void OnAttackPressed()
    {
        Debug.Log("Attack pressed!");
        // Implement attack logic here
    }
    
    private void OnInteractPressed()
    {
        Debug.Log("Interact pressed!");
        // Implement interaction logic here
    }
    
    #endregion
    
    void OnDestroy()
    {
        // Unsubscribe from input events
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
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        }
    }
}
