using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class Interactable : MonoBehaviour
{
    #region Serialized Fields
    [Header("Interaction Settings")]
    [SerializeField] protected string interactionPrompt = "Press E to interact";
    [SerializeField] protected float interactionDistance = 3f;
    [SerializeField] protected bool requiresHoldInteraction = false;
    [SerializeField] protected float holdDuration = 1f;
    
    [Header("Visual Feedback")]
    [SerializeField] protected bool showOutline = true;
    [SerializeField] protected Color outlineColor = Color.yellow;
    [SerializeField] protected float outlineWidth = 2f;
    #endregion

    #region Properties
    public string InteractionPrompt => interactionPrompt;
    public float InteractionDistance => interactionDistance;
    public bool RequiresHoldInteraction => requiresHoldInteraction;
    public float HoldDuration => holdDuration;
    public bool IsHighlighted { get; private set; }
    #endregion

    #region Events
    public event Action<Interactable> OnInteractionStart;
    public event Action<Interactable> OnInteractionEnd;
    public event Action<Interactable> OnHighlightStart;
    public event Action<Interactable> OnHighlightEnd;
    #endregion

    #region Private Fields
    protected bool isInteracting;
    protected Outline outline;
    #endregion
    #region Unity Lifecycle
    protected virtual void Awake()
    {
        InitializeOutline();
    }

    protected virtual void OnDestroy()
    {
        ClearEvents();
    }
    #endregion

    #region Initialization
    private void InitializeOutline()
    {
        if (!showOutline) return;
        
        outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = Vector2.one * outlineWidth;
        outline.enabled = false;
    }

    private void ClearEvents()
    {
        OnInteractionStart = null;
        OnInteractionEnd = null;
        OnHighlightStart = null;
        OnHighlightEnd = null;
    }
    #endregion
    #region Interaction Methods
    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    public virtual void Interact(GameObject player)
    {
        if (!CanInteract(player)) return;
        
        Debug.Log($"Interacted with {gameObject.name}");
        OnInteractionStart?.Invoke(this);
        
        PerformInteraction(player);
        
        OnInteractionEnd?.Invoke(this);
    }

    /// <summary>
    /// Override this method in derived classes to implement specific interaction logic
    /// </summary>
    protected abstract void PerformInteraction(GameObject player);

    /// <summary>
    /// Check if the player can interact with this object
    /// </summary>
    public virtual bool CanInteract(GameObject player) => true;

    /// <summary>
    /// Get the interaction prompt text to display to the player
    /// </summary>
    public virtual string GetInteractionPrompt() => interactionPrompt;

    /// <summary>
    /// Called during hold interaction to update progress
    /// </summary>
    public virtual void OnHoldProgress(float progress)
    {
        // Override in derived classes if needed
    }
    #endregion
    #region Highlighting
    /// <summary>
    /// Called when the player starts looking at this interactable
    /// </summary>
    public virtual void OnStartHighlight()
    {
        if (IsHighlighted) return;
        
        IsHighlighted = true;
        
        if (outline != null)
            outline.enabled = true;
        
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
            outline.enabled = false;
        
        OnHighlightEnd?.Invoke(this);
    }
    #endregion

    #region Debug Visualization
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
    #endregion
}