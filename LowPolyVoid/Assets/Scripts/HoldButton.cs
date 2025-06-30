using UnityEngine;

/// <summary>
/// A button that requires holding E for a certain duration
/// </summary>
public class HoldButton : Interactable
{
    [Header("Hold Button Settings")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool staysActivated = true; // If false, deactivates when released
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private Color activatedColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;
    
    [Header("Connected Objects")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private Light[] lightsToControl;
    
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private Renderer buttonRenderer;
    private AudioSource audioSource;
    private Material buttonMaterial;
    private Color originalColor;
    
    // Events
    public System.Action<bool> OnButtonStateChanged;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Set as hold interaction
        requiresHoldInteraction = true;
        interactionPrompt = $"Hold E for {holdDuration:F1}s to activate";
        
        // Store original position
        originalPosition = transform.localPosition;
        pressedPosition = originalPosition + Vector3.down * pressDepth;
        
        // Get components
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer != null)
        {
            buttonMaterial = buttonRenderer.material;
            originalColor = buttonMaterial.color;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Set initial state
        UpdateButtonState();
    }
    
    protected override void PerformInteraction(GameObject player)
    {
        ActivateButton();
    }
    
    public override void OnHoldProgress(float progress)
    {
        base.OnHoldProgress(progress);
        
        // Visual feedback during hold
        if (buttonRenderer != null)
        {
            Color currentColor = Color.Lerp(originalColor, activatedColor, progress);
            buttonMaterial.color = currentColor;
        }
        
        // Move button down gradually
        Vector3 currentPos = Vector3.Lerp(originalPosition, pressedPosition, progress);
        transform.localPosition = currentPos;
    }
    
    private void ActivateButton()
    {
        if (isActivated && staysActivated) return;
        
        isActivated = true;
        
        // Play sound
        if (pressSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pressSound);
        }
        
        // Update visual state
        UpdateButtonState();
        
        // Activate connected objects
        UpdateConnectedObjects();
        
        // Invoke event
        OnButtonStateChanged?.Invoke(isActivated);
        
        Debug.Log($"Button {gameObject.name} activated!");
        
        // If button doesn't stay activated, schedule deactivation
        if (!staysActivated)
        {
            Invoke(nameof(DeactivateButton), 2f); // Deactivate after 2 seconds
        }
    }
    
    private void DeactivateButton()
    {
        if (!isActivated) return;
        
        isActivated = false;
        
        // Play sound
        if (releaseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }
        
        // Update visual state
        UpdateButtonState();
        
        // Deactivate connected objects
        UpdateConnectedObjects();
        
        // Invoke event
        OnButtonStateChanged?.Invoke(isActivated);
        
        Debug.Log($"Button {gameObject.name} deactivated!");
    }
    
    private void UpdateButtonState()
    {
        // Update position
        transform.localPosition = isActivated ? pressedPosition : originalPosition;
        
        // Update color
        if (buttonRenderer != null)
        {
            Color targetColor = isActivated ? activatedColor : inactiveColor;
            buttonMaterial.color = targetColor;
        }
        
        // Update prompt
        if (isActivated && staysActivated)
        {
            interactionPrompt = "Button is activated";
        }
        else
        {
            interactionPrompt = $"Hold E for {holdDuration:F1}s to activate";
        }
    }
    
    private void UpdateConnectedObjects()
    {
        // Toggle connected GameObjects
        foreach (var obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(isActivated);
            }
        }
        
        // Control connected lights
        foreach (var light in lightsToControl)
        {
            if (light != null)
            {
                light.enabled = isActivated;
            }
        }
    }
    
    public override bool CanInteract(GameObject player)
    {
        // Can always interact unless it's permanently activated
        return !(isActivated && staysActivated);
    }
    
    /// <summary>
    /// Reset the button state (useful for puzzle resets)
    /// </summary>
    public void ResetButton()
    {
        CancelInvoke(); // Cancel any pending deactivation
        isActivated = false;
        UpdateButtonState();
        UpdateConnectedObjects();
        OnButtonStateChanged?.Invoke(isActivated);
    }
}
