using UnityEngine;

/// <summary>
/// A collectible item that can be picked up
/// </summary>
public class CollectibleItem : Interactable
{
    [Header("Collectible Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private int itemValue = 1;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private bool destroyOnPickup = true;
    
    private AudioSource audioSource;
    private bool hasBeenPickedUp = false;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Set interaction prompt
        interactionPrompt = $"Press E to pick up {itemName}";
        
        // Get audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    protected override void PerformInteraction(GameObject player)
    {
        if (hasBeenPickedUp) return;
        
        PickupItem(player);
    }
    
    public override bool CanInteract(GameObject player)
    {
        return !hasBeenPickedUp;
    }
    
    private void PickupItem(GameObject player)
    {
        hasBeenPickedUp = true;
        
        Debug.Log($"Picked up {itemName} (Value: {itemValue})");
        
        // Play pickup sound
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        
        // Spawn pickup effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, transform.rotation);
        }
        
        // Add to player inventory (you would implement this based on your inventory system)
        var playerInventory = player.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.AddItem(itemName, itemValue);
        }
        
        // Disable or destroy the item
        if (destroyOnPickup)
        {
            // Delay destruction slightly to allow sound to play
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0.1f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

/// <summary>
/// Simple inventory system for demonstration
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
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
    
    [SerializeField] private System.Collections.Generic.List<InventoryItem> items = new System.Collections.Generic.List<InventoryItem>();
    
    public void AddItem(string itemName, int quantity)
    {
        // Check if item already exists
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
    
    public bool HasItem(string itemName)
    {
        return GetItemQuantity(itemName) > 0;
    }
    
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
}
