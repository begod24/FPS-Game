using UnityEngine;

public enum GameState
{
    Playing,
    Paused,
    Menu,
    Inventory,
    Dialogue
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inventoryMenu;
    
    public GameState CurrentState { get; private set; } = GameState.Playing;
    
    // Events
    public System.Action<GameState> OnGameStateChanged;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Subscribe to input events
        if (InputManager.Instance != null)
        {
            // Example: Use Previous/Next for inventory navigation or weapon switching
            InputManager.Instance.OnPreviousPressed += OnPreviousWeapon;
            InputManager.Instance.OnNextPressed += OnNextWeapon;
        }
        
        if (UIInputManager.Instance != null)
        {
            UIInputManager.Instance.OnPausePressed += TogglePause;
        }
        
        // Start in playing state
        SetGameState(GameState.Playing);
    }
    
    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;
        
        GameState previousState = CurrentState;
        CurrentState = newState;
        
        // Handle state transitions
        HandleStateTransition(previousState, newState);
        
        // Notify listeners
        OnGameStateChanged?.Invoke(newState);
    }
    
    private void HandleStateTransition(GameState fromState, GameState toState)
    {
        // Disable input based on previous state
        switch (fromState)
        {
            case GameState.Playing:
                // Coming from playing state
                break;
            case GameState.Paused:
                // Coming from pause
                if (pauseMenu != null)
                    pauseMenu.SetActive(false);
                break;
            case GameState.Inventory:
                // Coming from inventory
                if (inventoryMenu != null)
                    inventoryMenu.SetActive(false);
                break;
        }
        
        // Enable input based on new state
        switch (toState)
        {
            case GameState.Playing:
                // Enable player input, disable UI
                InputManager.Instance?.EnablePlayerInput();
                UIInputManager.Instance?.DisableUIInput();
                
                // Resume time
                Time.timeScale = 1f;
                
                // Lock cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
                
            case GameState.Paused:
                // Disable player input, enable UI
                InputManager.Instance?.DisablePlayerInput();
                UIInputManager.Instance?.EnableUIInput();
                
                // Pause time
                Time.timeScale = 0f;
                
                // Show pause menu
                if (pauseMenu != null)
                    pauseMenu.SetActive(true);
                
                // Unlock cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
                
            case GameState.Menu:
                // Disable player input, enable UI
                InputManager.Instance?.DisablePlayerInput();
                UIInputManager.Instance?.EnableUIInput();
                
                // Unlock cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
                
            case GameState.Inventory:
                // Disable player movement but keep some inputs active
                InputManager.Instance?.DisablePlayerInput();
                UIInputManager.Instance?.EnableUIInput();
                
                // Show inventory
                if (inventoryMenu != null)
                    inventoryMenu.SetActive(true);
                
                // Unlock cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
                
            case GameState.Dialogue:
                // Disable player input, enable UI
                InputManager.Instance?.DisablePlayerInput();
                UIInputManager.Instance?.EnableUIInput();
                break;
        }
        
        Debug.Log($"Game state changed from {fromState} to {toState}");
    }
    
    #region Input Event Handlers
    
    private void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            SetGameState(GameState.Paused);
        }
        else if (CurrentState == GameState.Paused)
        {
            SetGameState(GameState.Playing);
        }
    }
    
    private void OnPreviousWeapon()
    {
        if (CurrentState == GameState.Playing)
        {
            Debug.Log("Previous weapon selected");
            // Implement weapon switching logic here
        }
    }
    
    private void OnNextWeapon()
    {
        if (CurrentState == GameState.Playing)
        {
            Debug.Log("Next weapon selected");
            // Implement weapon switching logic here
        }
    }
    
    #endregion
    
    #region Public Methods
    
    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
    }
    
    public void PauseGame()
    {
        SetGameState(GameState.Paused);
    }
    
    public void OpenInventory()
    {
        SetGameState(GameState.Inventory);
    }
    
    public void CloseInventory()
    {
        SetGameState(GameState.Playing);
    }
    
    public void StartDialogue()
    {
        SetGameState(GameState.Dialogue);
    }
    
    public void EndDialogue()
    {
        SetGameState(GameState.Playing);
    }
    
    public bool IsGameplayActive()
    {
        return CurrentState == GameState.Playing;
    }
    
    #endregion
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPreviousPressed -= OnPreviousWeapon;
            InputManager.Instance.OnNextPressed -= OnNextWeapon;
        }
        
        if (UIInputManager.Instance != null)
        {
            UIInputManager.Instance.OnPausePressed -= TogglePause;
        }
    }
}
