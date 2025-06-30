using UnityEngine;

/// <summary>
/// A lever that can be activated and stays in position
/// </summary>
public class Lever : Interactable
{
    [Header("Lever Settings")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private float pullAngle = 45f;
    [SerializeField] private float animationSpeed = 3f;
    [SerializeField] private AudioClip activateSound;
    [SerializeField] private AudioClip deactivateSound;
    
    [Header("Connected Objects")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private MonoBehaviour[] scriptsToToggle;
    
    private Quaternion inactiveRotation;
    private Quaternion activeRotation;
    private bool isAnimating = false;
    private AudioSource audioSource;
    
    // Events
    public System.Action<bool> OnLeverToggled;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Store rotations
        inactiveRotation = transform.rotation;
        activeRotation = inactiveRotation * Quaternion.Euler(pullAngle, 0, 0);
        
        // Get audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Set initial state
        UpdateLeverState();
        UpdateInteractionPrompt();
    }
    
    protected override void PerformInteraction(GameObject player)
    {
        if (isAnimating) return;
        
        ToggleLever();
    }
    
    public override bool CanInteract(GameObject player)
    {
        return !isAnimating;
    }
    
    private void ToggleLever()
    {
        isActivated = !isActivated;
        isAnimating = true;
        
        // Play sound
        AudioClip soundToPlay = isActivated ? activateSound : deactivateSound;
        if (soundToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundToPlay);
        }
        
        // Start animation
        StartCoroutine(AnimateLever());
        
        // Update connected objects
        UpdateConnectedObjects();
        
        // Update prompt
        UpdateInteractionPrompt();
        
        // Invoke event
        OnLeverToggled?.Invoke(isActivated);
    }
    
    private System.Collections.IEnumerator AnimateLever()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = isActivated ? activeRotation : inactiveRotation;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * animationSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            yield return null;
        }
        
        transform.rotation = targetRotation;
        isAnimating = false;
    }
    
    private void UpdateLeverState()
    {
        // Set initial rotation based on activation state
        transform.rotation = isActivated ? activeRotation : inactiveRotation;
        UpdateConnectedObjects();
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
        
        // Toggle connected scripts
        foreach (var script in scriptsToToggle)
        {
            if (script != null)
            {
                script.enabled = isActivated;
            }
        }
    }
    
    private void UpdateInteractionPrompt()
    {
        interactionPrompt = isActivated ? "Press E to deactivate lever" : "Press E to activate lever";
    }
    
    /// <summary>
    /// Set the lever state without animation (useful for loading saved states)
    /// </summary>
    /// <param name="activated">The new activation state</param>
    public void SetLeverState(bool activated)
    {
        if (isAnimating) return;
        
        isActivated = activated;
        UpdateLeverState();
        UpdateInteractionPrompt();
        OnLeverToggled?.Invoke(isActivated);
    }
}
