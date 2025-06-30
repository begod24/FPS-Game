using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] protected string interactionPrompt = "Press E to interact";
    [SerializeField] protected float interactionDistance = 3f;
    [SerializeField] protected bool requiresHoldInteraction = false;
    [SerializeField] protected float holdDuration = 1f;
    
    [Header("Visual Feedback")]
    [SerializeField] protected bool showOutline = true;
    [SerializeField] protected Color outlineColor = Color.yellow;
    [SerializeField] protected float outlineWidth = 2f;
    
    // Events
    public System.Action<Interactable> OnInteractionStart;
    public System.Action<Interactable> OnInteractionEnd;
    public System.Action<Interactable> OnHighlightStart;
    public System.Action<Interactable> OnHighlightEnd;
    
    // Properties
    public string InteractionPrompt => interactionPrompt;
    public float InteractionDistance => interactionDistance;
    public bool RequiresHoldInteraction => requiresHoldInteraction;
    public float HoldDuration => holdDuration;
    public bool IsHighlighted { get; private set; }
    
    protected bool isInteracting = false;
    protected Outline outline;
    
    protected virtual void Awake()
    {
        // Get or add outline component for visual feedback
        if (showOutline)
        {
            outline = GetComponent<Outline>();
            if (outline == null)
            {
                outline = gameObject.AddComponent<Outline>();
            }
            
            outline.effectColor = outlineColor;
            outline.effectDistance = Vector2.one * outlineWidth;
            outline.enabled = false;
        }
    }
    
    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    /// <param name="player">The player GameObject that is interacting</param>
    public virtual void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;
        
        Debug.Log($"Interacted with {gameObject.name}");
        OnInteractionStart?.Invoke(this);
        
        // Override this method in derived classes for specific interaction behavior
        PerformInteraction(player);
        
        OnInteractionEnd?.Invoke(this);
    }
    
    /// <summary>
    /// Override this method in derived classes to implement specific interaction logic
    /// </summary>
    /// <param name="player">The player GameObject that is interacting</param>
    protected abstract void PerformInteraction(GameObject player);
    
    /// <summary>
    /// Check if the player can interact with this object
    /// </summary>
    /// <param name="player">The player GameObject</param>
    /// <returns>True if interaction is allowed</returns>
    public virtual bool CanInteract(GameObject player)
    {
        // Override this method in derived classes for custom interaction conditions
        return true;
    }
    
    /// <summary>
    /// Called when the player starts looking at this interactable
    /// </summary>
    public virtual void OnStartHighlight()
    {
        if (IsHighlighted) return;
        
        IsHighlighted = true;
        
        if (outline != null)
        {
            outline.enabled = true;
        }
        
        OnHighlightStart?.Invoke(this);
    }
    
    /// <summary>
    /// Called when the player stops looking at this interactable
    /// </summary>
    public virtual void OnStopHighlight()
    {
        if (!IsHighlighted) return;
        
        IsHighlighted = false;
        
        if (outline != null)
        {
            outline.enabled = false;
        }
        
        OnHighlightEnd?.Invoke(this);
    }
    
    /// <summary>
    /// Called during hold interaction to update progress
    /// </summary>
    /// <param name="progress">Progress from 0 to 1</param>
    public virtual void OnHoldProgress(float progress)
    {
        // Override in derived classes if needed
    }
    
    /// <summary>
    /// Get the interaction prompt text to display to the player
    /// </summary>
    /// <returns>The prompt text</returns>
    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }
    
    protected virtual void OnDestroy()
    {
        OnInteractionStart = null;
        OnInteractionEnd = null;
        OnHighlightStart = null;
        OnHighlightEnd = null;
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw interaction range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}