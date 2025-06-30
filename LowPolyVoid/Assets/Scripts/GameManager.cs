using UnityEngine;
using System;

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
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inventoryMenu;
    #endregion

    #region Properties
    public GameState CurrentState { get; private set; } = GameState.Playing;
    #endregion

    #region Events
    public event Action<GameState> OnGameStateChanged;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    private void InitializeSingleton()
    {
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

    private void InitializeGame()
    {
        SubscribeToInputEvents();
        SetGameState(GameState.Playing);
    }

    private void SubscribeToInputEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPreviousPressed += OnPreviousWeapon;
            InputManager.Instance.OnNextPressed += OnNextWeapon;
        }
        
        if (UIInputManager.Instance != null)
        {
            UIInputManager.Instance.OnPausePressed += TogglePause;
        }
    }

    private void UnsubscribeFromEvents()
    {
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
    #endregion
    #region State Management
    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;
        
        GameState previousState = CurrentState;
        CurrentState = newState;
        
        HandleStateTransition(previousState, newState);
        OnGameStateChanged?.Invoke(newState);
        
        Debug.Log($"Game state changed from {previousState} to {newState}");
    }

    private void HandleStateTransition(GameState fromState, GameState toState)
    {
        ExitState(fromState);
        EnterState(toState);
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                if (pauseMenu != null) pauseMenu.SetActive(false);
                break;
            case GameState.Inventory:
                if (inventoryMenu != null) inventoryMenu.SetActive(false);
                break;
        }
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                SetupPlayingState();
                break;
            case GameState.Paused:
                SetupPausedState();
                break;
            case GameState.Menu:
                SetupMenuState();
                break;
            case GameState.Inventory:
                SetupInventoryState();
                break;
            case GameState.Dialogue:
                SetupDialogueState();
                break;
        }
    }

    private void SetupPlayingState()
    {
        InputManager.Instance?.EnablePlayerInput();
        UIInputManager.Instance?.DisableUIInput();
        
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetupPausedState()
    {
        InputManager.Instance?.DisablePlayerInput();
        UIInputManager.Instance?.EnableUIInput();
        
        Time.timeScale = 0f;
        
        if (pauseMenu != null) pauseMenu.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetupMenuState()
    {
        InputManager.Instance?.DisablePlayerInput();
        UIInputManager.Instance?.EnableUIInput();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetupInventoryState()
    {
        InputManager.Instance?.DisablePlayerInput();
        UIInputManager.Instance?.EnableUIInput();
        
        if (inventoryMenu != null) inventoryMenu.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetupDialogueState()
    {
        InputManager.Instance?.DisablePlayerInput();
        UIInputManager.Instance?.EnableUIInput();
    }
    #endregion
    
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
}
