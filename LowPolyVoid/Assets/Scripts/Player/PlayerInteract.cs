using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayerMask = -1;
    [SerializeField] private Transform interactionOrigin; // Usually the camera
    
    [Header("UI References")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Image holdProgressBar;
    [SerializeField] private GameObject holdProgressUI;
    
    [Header("Interaction Feedback")]
    [SerializeField] private bool enableInteractionSound = true;
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioSource audioSource;
    
    // Private variables
    private Interactable currentInteractable;
    private bool isHoldingInteraction = false;
    private float holdTimer = 0f;
    private Camera playerCamera;
    
    // Events
    public System.Action<Interactable> OnInteractionStart;
    public System.Action<Interactable> OnInteractionComplete;
    public System.Action<Interactable> OnInteractionCancel;

    private PlayerUI playerUI;
    
    void Start()
    {
        // Get player UI component
        playerUI = GetComponent<PlayerUI>();
        // Get camera reference
        if (interactionOrigin == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                playerCamera = FindObjectOfType<Camera>();

            if (playerCamera != null)
                interactionOrigin = playerCamera.transform;
        }
        
        // Get audio source if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Initialize UI
        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(false);
        if (holdProgressUI != null)
            holdProgressUI.SetActive(false);
        
        // Subscribe to input events
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed += StartInteraction;
            InputManager.Instance.OnInteractReleased += StopInteraction;
        }
    }

    void Update()
    {
        CheckForInteractables();
        HandleHoldInteraction();
    }
    
    private void CheckForInteractables()
    {
        if (interactionOrigin == null) return;
        
        // Perform raycast from interaction origin
        Ray ray = new Ray(interactionOrigin.position, interactionOrigin.forward);
        RaycastHit hit;
        
        bool hitInteractable = false;
        
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayerMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable != null)
            {
                hitInteractable = true;
                
                // Check if we're looking at a new interactable
                if (currentInteractable != interactable)
                {
                    // Stop highlighting previous interactable
                    if (currentInteractable != null)
                        currentInteractable.OnStopHighlight();
                    
                    // Start highlighting new interactable
                    currentInteractable = interactable;
                    currentInteractable.OnStartHighlight();
                    
                    // Update UI
                    ShowInteractionPrompt(currentInteractable.GetInteractionPrompt());
                }
            }
        }
        
        // If we're not looking at an interactable anymore
        if (!hitInteractable && currentInteractable != null)
        {
            currentInteractable.OnStopHighlight();
            currentInteractable = null;
            HideInteractionPrompt();
            CancelHoldInteraction();
        }
        
        // Debug ray
        Debug.DrawRay(interactionOrigin.position, interactionOrigin.forward * interactionRange, 
                     currentInteractable != null ? Color.green : Color.red);
    }
    
    private void StartInteraction()
    {
        if (currentInteractable == null) return;
        
        // Check if we can interact
        if (!currentInteractable.CanInteract(gameObject)) return;
        
        OnInteractionStart?.Invoke(currentInteractable);
        
        if (currentInteractable.RequiresHoldInteraction)
        {
            // Start hold interaction
            isHoldingInteraction = true;
            holdTimer = 0f;
            
            if (holdProgressUI != null)
                holdProgressUI.SetActive(true);
        }
        else
        {
            // Immediate interaction
            PerformInteraction();
        }
    }
    
    private void StopInteraction()
    {
        if (!isHoldingInteraction) return;
        
        // Cancel hold interaction if not completed
        CancelHoldInteraction();
    }
    
    private void HandleHoldInteraction()
    {
        if (!isHoldingInteraction || currentInteractable == null) return;
        
        holdTimer += Time.deltaTime;
        float progress = holdTimer / currentInteractable.HoldDuration;
        
        // Update progress bar
        if (holdProgressBar != null)
            holdProgressBar.fillAmount = progress;
        
        // Update interactable with progress
        currentInteractable.OnHoldProgress(progress);
        
        // Check if hold is complete
        if (progress >= 1f)
        {
            PerformInteraction();
            isHoldingInteraction = false;
            
            if (holdProgressUI != null)
                holdProgressUI.SetActive(false);
        }
    }
    
    private void PerformInteraction()
    {
        if (currentInteractable == null) return;
        
        // Play interaction sound
        if (enableInteractionSound && interactionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
        
        // Perform the interaction
        currentInteractable.Interact(gameObject);
        
        OnInteractionComplete?.Invoke(currentInteractable);
        
        // Reset UI
        if (holdProgressUI != null)
            holdProgressUI.SetActive(false);
    }
    
    private void CancelHoldInteraction()
    {
        if (!isHoldingInteraction) return;
        
        isHoldingInteraction = false;
        holdTimer = 0f;
        
        if (holdProgressUI != null)
            holdProgressUI.SetActive(false);
        
        if (holdProgressBar != null)
            holdProgressBar.fillAmount = 0f;
        
        OnInteractionCancel?.Invoke(currentInteractable);
    }
    
    private void ShowInteractionPrompt(string promptText)
    {
        if (interactionPromptUI == null) return;
        
        interactionPromptUI.SetActive(true);
        
        if (this.promptText != null)
            this.promptText.text = promptText;
    }
    
    private void HideInteractionPrompt()
    {
        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(false);
    }
    
    #region Public Methods
    
    /// <summary>
    /// Force interact with a specific interactable (useful for scripted interactions)
    /// </summary>
    /// <param name="interactable">The interactable to interact with</param>
    public void ForceInteract(Interactable interactable)
    {
        if (interactable != null && interactable.CanInteract(gameObject))
        {
            interactable.Interact(gameObject);
        }
    }
    
    /// <summary>
    /// Get the currently highlighted interactable
    /// </summary>
    /// <returns>The current interactable or null</returns>
    public Interactable GetCurrentInteractable()
    {
        return currentInteractable;
    }
    
    /// <summary>
    /// Set the interaction range
    /// </summary>
    /// <param name="range">New interaction range</param>
    public void SetInteractionRange(float range)
    {
        interactionRange = range;
    }
    
    /// <summary>
    /// Enable or disable interaction
    /// </summary>
    /// <param name="enabled">Whether interaction should be enabled</param>
    public void SetInteractionEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (!enabled)
        {
            // Clean up when disabled
            if (currentInteractable != null)
            {
                currentInteractable.OnStopHighlight();
                currentInteractable = null;
            }
            HideInteractionPrompt();
            CancelHoldInteraction();
        }
    }
    
    #endregion
    
    void OnDestroy()
    {
        // Unsubscribe from input events
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed -= StartInteraction;
            InputManager.Instance.OnInteractReleased -= StopInteraction;
        }
        
        // Clean up events
        OnInteractionStart = null;
        OnInteractionComplete = null;
        OnInteractionCancel = null;
    }
    
    void OnDrawGizmosSelected()
    {
        if (interactionOrigin != null)
        {
            // Draw interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(interactionOrigin.position, interactionOrigin.forward * interactionRange);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(interactionOrigin.position + interactionOrigin.forward * interactionRange, 0.1f);
        }
    }
}
