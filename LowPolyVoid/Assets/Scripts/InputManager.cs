using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerInputActions;
    
    private InputActionMap playerActionMap;
    
    // Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction attackAction;
    private InputAction interactAction;
    private InputAction previousAction;
    private InputAction nextAction;
    
    // Input Values
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintPressed { get; private set; }
    public bool CrouchPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool PreviousPressed { get; private set; }
    public bool NextPressed { get; private set; }
    
    // Events
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnCrouchPressed;
    public event Action OnCrouchReleased;
    public event Action OnAttackPressed;
    public event Action OnAttackReleased;
    public event Action OnInteractPressed;
    public event Action OnInteractReleased;
    public event Action OnPreviousPressed;
    public event Action OnNextPressed;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeInputs()
    {
        // Get the player action map
        playerActionMap = playerInputActions.FindActionMap("Player");
        
        // Get individual actions
        moveAction = playerActionMap.FindAction("Move");
        lookAction = playerActionMap.FindAction("Look");
        jumpAction = playerActionMap.FindAction("Jump");
        sprintAction = playerActionMap.FindAction("Sprint");
        crouchAction = playerActionMap.FindAction("Crouch");
        attackAction = playerActionMap.FindAction("Attack");
        interactAction = playerActionMap.FindAction("Interact");
        previousAction = playerActionMap.FindAction("Previous");
        nextAction = playerActionMap.FindAction("Next");
        
        // Subscribe to input events
        SubscribeToInputEvents();
    }
    
    private void SubscribeToInputEvents()
    {
        // Movement and Look are continuous actions
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        
        lookAction.performed += OnLookPerformed;
        lookAction.canceled += OnLookCanceled;
        
        // Jump
        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;
        
        // Sprint
        sprintAction.performed += OnSprintPerformed;
        sprintAction.canceled += OnSprintCanceled;
        
        // Crouch
        crouchAction.performed += OnCrouchPerformed;
        crouchAction.canceled += OnCrouchCanceled;
        
        // Attack
        attackAction.performed += OnAttackPerformed;
        attackAction.canceled += OnAttackCanceled;
        
        // Interact
        interactAction.performed += OnInteractPerformed;
        interactAction.canceled += OnInteractCanceled;
        
        // Previous/Next
        previousAction.performed += OnPreviousPerformed;
        nextAction.performed += OnNextPerformed;
    }
    
    private void UnsubscribeFromInputEvents()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
        }
        
        if (lookAction != null)
        {
            lookAction.performed -= OnLookPerformed;
            lookAction.canceled -= OnLookCanceled;
        }
        
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJumpPerformed;
            jumpAction.canceled -= OnJumpCanceled;
        }
        
        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;
        }
        
        if (crouchAction != null)
        {
            crouchAction.performed -= OnCrouchPerformed;
            crouchAction.canceled -= OnCrouchCanceled;
        }
        
        if (attackAction != null)
        {
            attackAction.performed -= OnAttackPerformed;
            attackAction.canceled -= OnAttackCanceled;
        }
        
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            interactAction.canceled -= OnInteractCanceled;
        }
        
        if (previousAction != null)
            previousAction.performed -= OnPreviousPerformed;
            
        if (nextAction != null)
            nextAction.performed -= OnNextPerformed;
    }
    
    #region Input Event Handlers
    
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }
    
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MovementInput = Vector2.zero;
    }
    
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }
    
    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        LookInput = Vector2.zero;
    }
    
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        JumpPressed = true;
        OnJumpPressed?.Invoke();
    }
    
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        JumpPressed = false;
        OnJumpReleased?.Invoke();
    }
    
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        SprintPressed = true;
        OnSprintPressed?.Invoke();
    }
    
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        SprintPressed = false;
        OnSprintReleased?.Invoke();
    }
    
    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        CrouchPressed = true;
        OnCrouchPressed?.Invoke();
    }
    
    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        CrouchPressed = false;
        OnCrouchReleased?.Invoke();
    }
    
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        AttackPressed = true;
        OnAttackPressed?.Invoke();
    }
    
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        AttackPressed = false;
        OnAttackReleased?.Invoke();
    }
    
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        InteractPressed = true;
        OnInteractPressed?.Invoke();
    }
    
    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        InteractPressed = false;
        OnInteractReleased?.Invoke();
    }
    
    private void OnPreviousPerformed(InputAction.CallbackContext context)
    {
        PreviousPressed = true;
        OnPreviousPressed?.Invoke();
    }
    
    private void OnNextPerformed(InputAction.CallbackContext context)
    {
        NextPressed = true;
        OnNextPressed?.Invoke();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Enable player input
    /// </summary>
    public void EnablePlayerInput()
    {
        playerActionMap?.Enable();
    }
    
    /// <summary>
    /// Disable player input
    /// </summary>
    public void DisablePlayerInput()
    {
        playerActionMap?.Disable();
    }
    
    /// <summary>
    /// Toggle player input on/off
    /// </summary>
    public void TogglePlayerInput()
    {
        if (playerActionMap.enabled)
            DisablePlayerInput();
        else
            EnablePlayerInput();
    }
    
    /// <summary>
    /// Reset one-time input flags (call this after processing input)
    /// </summary>
    public void ResetInputFlags()
    {
        PreviousPressed = false;
        NextPressed = false;
    }
    
    /// <summary>
    /// Get the current input device being used
    /// </summary>
    public string GetCurrentInputDevice()
    {
        var lastDevice = InputSystem.GetDevice<InputDevice>();
        return lastDevice?.displayName ?? "Unknown";
    }
    
    /// <summary>
    /// Check if gamepad is connected
    /// </summary>
    public bool IsGamepadConnected()
    {
        return Gamepad.current != null && Gamepad.current.added;
    }
    
    #endregion
    
    void OnEnable()
    {
        EnablePlayerInput();
    }
    
    void OnDisable()
    {
        DisablePlayerInput();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromInputEvents();
        DisablePlayerInput();
    }
}