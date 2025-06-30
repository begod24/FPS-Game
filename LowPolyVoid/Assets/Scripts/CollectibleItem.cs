using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// A collectible item that can be picked up
/// </summary>
public class CollectibleItem : Interactable
{
    #region Serialized Fields
    [Header("Collectible Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private int itemValue = 1;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private bool destroyOnPickup = true;
    #endregion

    #region Private Fields
    private AudioSource audioSource;
    private bool hasBeenPickedUp;
    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        InitializeCollectible();
    }
    #endregion

    #region Initialization
    private void InitializeCollectible()
    {
        interactionPrompt = $"Press E to pick up {itemName}";
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }
    #endregion

    #region Interaction Implementation
    protected override void PerformInteraction(GameObject player)
    {
        if (!hasBeenPickedUp)
        {
            PickupItem(player);
        }
    }

    public override bool CanInteract(GameObject player) => !hasBeenPickedUp;
    #endregion

    #region Item Pickup
    private void PickupItem(GameObject player)
    {
        hasBeenPickedUp = true;
        
        Debug.Log($"Picked up {itemName} (Value: {itemValue})");
        
        PlayPickupSound();
        SpawnPickupEffect();
        AddToPlayerInventory(player);
        HandleItemDestruction();
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }

    private void SpawnPickupEffect()
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, transform.rotation);
        }
    }

    private void AddToPlayerInventory(GameObject player)
    {
        var playerInventory = player.GetComponent<PlayerInventory>();
        playerInventory?.AddItem(itemName, itemValue);
    }

    private void HandleItemDestruction()
    {
        if (destroyOnPickup)
        {
            float destroyDelay = pickupSound != null ? pickupSound.length : 0.1f;
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}

/// <summary>
/// Simple inventory system for demonstration
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    #region Nested Classes
    [Serializable]
    public class InventoryItem
    {
        public string name;
        public int quantity;
        
        public InventoryItem(string itemName, int itemQuantity)
        {
            name = itemName;
            quantity = itemQuantity;
        }
    }
    #endregion

    #region Serialized Fields
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    #endregion

    #region Public API
    public void AddItem(string itemName, int quantity)
    {
        var existingItem = items.Find(item => item.name == itemName);
        
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new InventoryItem(itemName, quantity));
        }
        
        Debug.Log($"Added {quantity} {itemName} to inventory. Total: {GetItemQuantity(itemName)}");
    }
    
    public int GetItemQuantity(string itemName)
    {
        var item = items.Find(i => i.name == itemName);
        return item?.quantity ?? 0;
    }
    
    public bool HasItem(string itemName) => GetItemQuantity(itemName) > 0;
    
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        var item = items.Find(i => i.name == itemName);
        
        if (item != null && item.quantity >= quantity)
        {
            item.quantity -= quantity;
            
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
            
            return true;
        }
        
        return false;
    }
    #endregion
}
