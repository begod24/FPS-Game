# Interaction System Setup Guide

This comprehensive interaction system allows players to interact with objects by pressing E (or the configured interact key).

## Core Components:

### 1. **Interactable.cs** (Base Class)
- Abstract base class for all interactable objects
- Handles highlighting, interaction prompts, and hold interactions
- Automatic outline creation for visual feedback

### 2. **PlayerInteract.cs** (Player Component)
- Handles raycast detection of interactables
- Manages interaction input (E key)
- Shows/hides UI prompts
- Supports both instant and hold interactions

### 3. **Example Interactable Classes:**
- **Door.cs** - Doors that open/close
- **CollectibleItem.cs** - Items that can be picked up
- **Lever.cs** - Levers that activate/deactivate objects
- **HoldButton.cs** - Buttons requiring hold interaction

## Setup Instructions:

### Player Setup:
1. Add `PlayerInteract` component to your player GameObject
2. Assign the camera transform to "Interaction Origin" (usually Main Camera)
3. Set the interaction range (default: 3 units)
4. Configure the interaction layer mask if needed

### UI Setup:
Create a UI Canvas with these elements:

1. **Interaction Prompt UI:**
   ```
   Canvas
   └── InteractionPrompt (GameObject)
       ├── Background (Image)
       └── PromptText (TextMeshPro)
   ```

2. **Hold Progress UI (for hold interactions):**
   ```
   Canvas
   └── HoldProgressUI (GameObject)
       ├── Background (Image)
       └── ProgressBar (Image with Fill type)
   ```

3. Assign these UI elements to the PlayerInteract component

### Creating Interactable Objects:

#### Basic Door:
1. Create a door GameObject with a Collider
2. Add the `Door` script
3. Configure open angle and animation speed
4. Add audio clips for open/close sounds

#### Collectible Item:
1. Create an item GameObject with a Collider
2. Add the `CollectibleItem` script
3. Set item name and value
4. Add pickup sound and effects
5. Add `PlayerInventory` component to player for item collection

#### Lever:
1. Create a lever GameObject with a Collider
2. Add the `Lever` script
3. Configure pull angle and connected objects
4. Assign GameObjects to activate when lever is pulled

#### Hold Button:
1. Create a button GameObject with a Collider
2. Add the `HoldButton` script
3. Set hold duration and visual settings
4. Configure connected objects and lights

### Creating Custom Interactables:

```csharp
public class MyCustomInteractable : Interactable
{
    protected override void PerformInteraction(GameObject player)
    {
        // Your custom interaction logic here
        Debug.Log("Custom interaction performed!");
    }
    
    public override bool CanInteract(GameObject player)
    {
        // Custom conditions for interaction
        return base.CanInteract(player);
    }
}
```

## Configuration Options:

### Interactable Base Settings:
- **Interaction Prompt**: Text shown to player
- **Interaction Distance**: How close player needs to be
- **Requires Hold**: Whether interaction needs to be held
- **Hold Duration**: Time required for hold interaction
- **Show Outline**: Visual highlight when looking at object
- **Outline Color/Width**: Appearance of highlight

### PlayerInteract Settings:
- **Interaction Range**: Raycast distance
- **Interaction Layer Mask**: Which layers to check
- **Interaction Origin**: Transform to cast ray from (camera)
- **UI References**: Prompt and progress UI elements
- **Audio Settings**: Interaction sounds

## Key Features:

✅ **Raycast-based detection** - No need for triggers
✅ **Visual feedback** - Automatic outline highlighting
✅ **UI integration** - Customizable interaction prompts
✅ **Hold interactions** - Support for timed interactions
✅ **Audio support** - Interaction sound effects
✅ **Event system** - Subscribe to interaction events
✅ **Modular design** - Easy to extend with new interactables
✅ **Performance optimized** - Efficient raycast checking

## Input Integration:

The system automatically integrates with the InputManager:
- Uses `OnInteractPressed` and `OnInteractReleased` events
- Supports both tap and hold interactions
- Works with keyboard and gamepad input

## Debugging:

- Green ray in Scene view = Looking at interactable
- Red ray in Scene view = No interactable in range
- Blue wireframe = Interaction range visualization
- Console logs for all interactions

## Example Usage:

```csharp
// Subscribe to interaction events
playerInteract.OnInteractionComplete += (interactable) => {
    Debug.Log($"Interacted with: {interactable.name}");
};

// Force interaction from code
playerInteract.ForceInteract(someInteractable);

// Check current interactable
Interactable current = playerInteract.GetCurrentInteractable();
```

## Tips:

1. Place interactable objects on appropriate layers
2. Ensure colliders are properly sized for detection
3. Test interaction ranges in play mode
4. Use the gizmos to visualize interaction ranges
5. Consider adding audio feedback for better user experience
6. Use events to trigger other game systems when objects are interacted with
