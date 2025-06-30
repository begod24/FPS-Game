using UnityEngine;

/// <summary>
/// A simple door that can be opened and closed
/// </summary>
public class Door : Interactable
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating = false;
    private AudioSource audioSource;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Store initial rotation as closed position
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        
        // Get audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Update interaction prompt based on current state
        UpdateInteractionPrompt();
    }
    
    protected override void PerformInteraction(GameObject player)
    {
        if (isAnimating) return;
        
        ToggleDoor();
    }
    
    public override bool CanInteract(GameObject player)
    {
        // Can't interact while door is animating
        return !isAnimating;
    }
    
    private void ToggleDoor()
    {
        isOpen = !isOpen;
        isAnimating = true;
        
        // Play sound
        AudioClip soundToPlay = isOpen ? openSound : closeSound;
        if (soundToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundToPlay);
        }
        
        // Start animation
        StartCoroutine(AnimateDoor());
        
        // Update prompt
        UpdateInteractionPrompt();
    }
    
    private System.Collections.IEnumerator AnimateDoor()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        
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
    
    private void UpdateInteractionPrompt()
    {
        interactionPrompt = isOpen ? "Press E to close door" : "Press E to open door";
    }
}
