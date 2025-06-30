using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class UIInputManager : MonoBehaviour
{
    #region Singleton
    public static UIInputManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset inputActions;
    #endregion

    #region Input Actions
    private InputActionMap uiActionMap;
    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction cancelAction;
    private InputAction pauseAction;
    #endregion

    #region Input Properties
    public Vector2 NavigateInput { get; private set; }
    public bool SubmitPressed { get; private set; }
    public bool CancelPressed { get; private set; }
    public bool PausePressed { get; private set; }
    #endregion

    #region Events
    public event Action OnSubmitPressed;
    public event Action OnCancelPressed;
    public event Action OnPausePressed;
    public event Action<Vector2> OnNavigate;
    #endregion
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUIInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeUIInputs()
    {
        if (inputActions == null)
        {
            Debug.LogWarning("Input Actions asset is not assigned in UIInputManager!");
            return;
        }
        
        // Try to get UI action map (you may need to create this in your Input Actions)
        uiActionMap = inputActions.FindActionMap("UI");
        
        if (uiActionMap != null)
        {
            // Get UI actions
            navigateAction = uiActionMap.FindAction("Navigate");
            submitAction = uiActionMap.FindAction("Submit");
            cancelAction = uiActionMap.FindAction("Cancel");
            pauseAction = uiActionMap.FindAction("Pause");
            
            // Subscribe to events
            SubscribeToUIEvents();
        }
    }
    
    private void SubscribeToUIEvents()
    {
        if (navigateAction != null)
        {
            navigateAction.performed += OnNavigatePerformed;
            navigateAction.canceled += OnNavigateCanceled;
        }
        
        if (submitAction != null)
        {
            submitAction.performed += OnSubmitPerformed;
            submitAction.canceled += OnSubmitCanceled;
        }
        
        if (cancelAction != null)
        {
            cancelAction.performed += OnCancelPerformed;
            cancelAction.canceled += OnCancelCanceled;
        }
        
        if (pauseAction != null)
        {
            pauseAction.performed += OnPausePerformed;
        }
    }
    
    private void UnsubscribeFromUIEvents()
    {
        if (navigateAction != null)
        {
            navigateAction.performed -= OnNavigatePerformed;
            navigateAction.canceled -= OnNavigateCanceled;
        }
        
        if (submitAction != null)
        {
            submitAction.performed -= OnSubmitPerformed;
            submitAction.canceled -= OnSubmitCanceled;
        }
        
        if (cancelAction != null)
        {
            cancelAction.performed -= OnCancelPerformed;
            cancelAction.canceled -= OnCancelCanceled;
        }
        
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
        }
    }
    
    #region UI Input Event Handlers
    
    private void OnNavigatePerformed(InputAction.CallbackContext context)
    {
        NavigateInput = context.ReadValue<Vector2>();
        OnNavigate?.Invoke(NavigateInput);
    }
    
    private void OnNavigateCanceled(InputAction.CallbackContext context)
    {
        NavigateInput = Vector2.zero;
    }
    
    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        SubmitPressed = true;
        OnSubmitPressed?.Invoke();
    }
    
    private void OnSubmitCanceled(InputAction.CallbackContext context)
    {
        SubmitPressed = false;
    }
    
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        CancelPressed = true;
        OnCancelPressed?.Invoke();
    }
    
    private void OnCancelCanceled(InputAction.CallbackContext context)
    {
        CancelPressed = false;
    }
    
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        PausePressed = true;
        OnPausePressed?.Invoke();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Enable UI input
    /// </summary>
    public void EnableUIInput()
    {
        uiActionMap?.Enable();
    }
    
    /// <summary>
    /// Disable UI input
    /// </summary>
    public void DisableUIInput()
    {
        uiActionMap?.Disable();
    }
    
    /// <summary>
    /// Reset UI input flags
    /// </summary>
    public void ResetUIInputFlags()
    {
        SubmitPressed = false;
        CancelPressed = false;
        PausePressed = false;
    }
    
    #endregion
    
    void OnEnable()
    {
        EnableUIInput();
    }
    
    void OnDisable()
    {
        DisableUIInput();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromUIEvents();
        DisableUIInput();
    }
}
