using UnityEngine;
using System.Collections;

/// <summary>
/// A simple door that can be opened and closed
/// </summary>
public class Door : Interactable
{
    #region Serialized Fields
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    #endregion

    #region Private Fields
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating;
    private AudioSource audioSource;
    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        InitializeDoor();
    }
    #endregion

    #region Initialization
    private void InitializeDoor()
    {
        SetupRotations();
        SetupAudioSource();
        UpdateInteractionPrompt();
    }

    private void SetupRotations()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }
    #endregion

    #region Interaction Implementation
    protected override void PerformInteraction(GameObject player)
    {
        if (!isAnimating)
        {
            ToggleDoor();
        }
    }

    public override bool CanInteract(GameObject player) => !isAnimating;
    #endregion

    #region Door Control
    private void ToggleDoor()
    {
        isOpen = !isOpen;
        isAnimating = true;
        
        PlayDoorSound();
        StartCoroutine(AnimateDoor());
        UpdateInteractionPrompt();
    }

    private void PlayDoorSound()
    {
        AudioClip soundToPlay = isOpen ? openSound : closeSound;
        if (soundToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundToPlay);
        }
    }

    private IEnumerator AnimateDoor()
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
    #endregion
}
