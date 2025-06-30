using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInteract : MonoBehaviour
{
    #region Serialized Fields
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayerMask = -1;
    [SerializeField] private Transform interactionOrigin;
    
    [Header("UI References")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Image holdProgressBar;
    [SerializeField] private GameObject holdProgressUI;
    
    [Header("Audio")]
    [SerializeField] private AudioClip interactionSound;
    #endregion

    #region Private Fields
    private Interactable currentInteractable;
    private bool isHoldingInteraction;
    private float holdTimer;
    private AudioSource audioSource;
    #endregion

    #region Events
    public event Action<Interactable> OnInteractionStart;
    public event Action<Interactable> OnInteractionComplete;
    public event Action<Interactable> OnInteractionCancel;
    #endregion
    #region Unity Lifecycle
    private void Start()
    {
        InitializeComponents();
        SetupInputEvents();
        InitializeUI();
    }

    private void Update()
    {
        CheckForInteractables();
        HandleHoldInteraction();
    }

    private void OnDestroy()
    {
        UnsubscribeFromInputEvents();
        ClearEvents();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        SetupInteractionOrigin();
        SetupAudioSource();
    }

    private void SetupInteractionOrigin()
    {
        if (interactionOrigin == null)
        {
            Camera playerCamera = Camera.main ?? FindFirstObjectByType<Camera>();
            if (playerCamera != null)
                interactionOrigin = playerCamera.transform;
        }
    }

    private void SetupAudioSource()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    private void SetupInputEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed += StartInteraction;
            InputManager.Instance.OnInteractReleased += StopInteraction;
        }
    }

    private void InitializeUI()
    {
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
        if (holdProgressUI != null) holdProgressUI.SetActive(false);
    }

    private void UnsubscribeFromInputEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed -= StartInteraction;
            InputManager.Instance.OnInteractReleased -= StopInteraction;
        }
    }

    private void ClearEvents()
    {
        OnInteractionStart = null;
        OnInteractionComplete = null;
        OnInteractionCancel = null;
    }
    #endregion
    #region Interaction Detection
    private void CheckForInteractables()
    {
        if (interactionOrigin == null) return;
        
        Ray ray = new Ray(interactionOrigin.position, interactionOrigin.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayerMask) 
            && hit.collider.TryGetComponent<Interactable>(out Interactable interactable))
        {
            HandleNewInteractable(interactable);
        }
        else
        {
            ClearCurrentInteractable();
        }
        
        DrawDebugRay();
    }

    private void HandleNewInteractable(Interactable newInteractable)
    {
        if (currentInteractable == newInteractable) return;
        
        ClearCurrentInteractable();
        
        currentInteractable = newInteractable;
        currentInteractable.OnStartHighlight();
        ShowInteractionPrompt(currentInteractable.GetInteractionPrompt());
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable == null) return;
        
        currentInteractable.OnStopHighlight();
        currentInteractable = null;
        HideInteractionPrompt();
        CancelHoldInteraction();
    }

    private void DrawDebugRay()
    {
        Color rayColor = currentInteractable != null ? Color.green : Color.red;
        Debug.DrawRay(interactionOrigin.position, interactionOrigin.forward * interactionRange, rayColor);
    }
    #endregion
    #region Interaction Handling
    private void StartInteraction()
    {
        if (!CanStartInteraction()) return;
        
        OnInteractionStart?.Invoke(currentInteractable);
        
        if (currentInteractable.RequiresHoldInteraction)
        {
            StartHoldInteraction();
        }
        else
        {
            PerformInteraction();
        }
    }

    private void StopInteraction()
    {
        if (isHoldingInteraction)
        {
            CancelHoldInteraction();
        }
    }

    private bool CanStartInteraction()
    {
        return currentInteractable != null && currentInteractable.CanInteract(gameObject);
    }

    private void StartHoldInteraction()
    {
        isHoldingInteraction = true;
        holdTimer = 0f;
        
        if (holdProgressUI != null)
            holdProgressUI.SetActive(true);
    }

    private void HandleHoldInteraction()
    {
        if (!isHoldingInteraction || currentInteractable == null) return;
        
        holdTimer += Time.deltaTime;
        float progress = holdTimer / currentInteractable.HoldDuration;
        
        UpdateHoldProgress(progress);
        currentInteractable.OnHoldProgress(progress);
        
        if (progress >= 1f)
        {
            CompleteHoldInteraction();
        }
    }

    private void UpdateHoldProgress(float progress)
    {
        if (holdProgressBar != null)
            holdProgressBar.fillAmount = progress;
    }

    private void CompleteHoldInteraction()
    {
        PerformInteraction();
        isHoldingInteraction = false;
        
        if (holdProgressUI != null)
            holdProgressUI.SetActive(false);
    }

    private void PerformInteraction()
    {
        if (currentInteractable == null) return;
        
        PlayInteractionSound();
        currentInteractable.Interact(gameObject);
        OnInteractionComplete?.Invoke(currentInteractable);
        
        if (holdProgressUI != null)
            holdProgressUI.SetActive(false);
    }

    private void PlayInteractionSound()
    {
        if (interactionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
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
    #endregion
    #region UI Management
    private void ShowInteractionPrompt(string prompt)
    {
        if (interactionPromptUI == null) return;
        
        interactionPromptUI.SetActive(true);
        
        if (promptText != null)
            promptText.text = prompt;
    }
    
    private void HideInteractionPrompt()
    {
        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(false);
    }
    #endregion

    #region Public API
    /// <summary>
    /// Force interact with a specific interactable
    /// </summary>
    public void ForceInteract(Interactable interactable)
    {
        if (interactable?.CanInteract(gameObject) == true)
        {
            interactable.Interact(gameObject);
        }
    }
    
    /// <summary>
    /// Get the currently highlighted interactable
    /// </summary>
    public Interactable GetCurrentInteractable() => currentInteractable;
    
    /// <summary>
    /// Set the interaction range
    /// </summary>
    public void SetInteractionRange(float range) => interactionRange = range;
    
    /// <summary>
    /// Enable or disable interaction system
    /// </summary>
    public void SetInteractionEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (!enabled)
        {
            ClearCurrentInteractable();
        }
    }
    #endregion
    #region Debug Visualization
    private void OnDrawGizmosSelected()
    {
        if (interactionOrigin == null) return;
        
        // Draw interaction range
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(interactionOrigin.position, interactionOrigin.forward * interactionRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactionOrigin.position + interactionOrigin.forward * interactionRange, 0.1f);
    }
    #endregion
}
