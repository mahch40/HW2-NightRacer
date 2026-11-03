using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, GameOver }

// Singleton that owns the game state (Menu, Playing, Paused, GameOver), the score,
// pausing and restarting. Everything else reacts to its events.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Difficulty difficulty;

    [Header("Setup")]
    [SerializeField] private GameState initialState = GameState.Playing;
    [SerializeField] private float scorePerUnitDistance = 0.1f;
    [SerializeField] private int mainMenuSceneIndex = 0;

    public event Action<GameState> OnStateChanged;
    public event Action<int> OnScoreChanged;

    public GameState State { get; private set; }
    public int Score => distanceScore + bonusScore;

    private GameInput input;
    private int distanceScore;
    private int bonusScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("More than one GameManager in the scene. Disabling the extra one.");
            enabled = false;
            return;
        }

        Instance = this;
        input = new GameInput();
    }

    private void OnEnable()
    {
        if (Instance != this) return;
        input.Driving.Enable();
        input.Driving.Pause.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        if (Instance != this) return;
        input.Driving.Pause.performed -= OnPausePressed;
        input.Driving.Disable();
    }

    private void Start()
    {
        if (playerHealth != null) playerHealth.OnDied += HandlePlayerDied;
        ApplyState(initialState);
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        if (playerHealth != null) playerHealth.OnDied -= HandlePlayerDied;
        Time.timeScale = 1f;   // never leave the editor frozen
        Instance = null;
    }

    private void Update()
    {
        if (State != GameState.Playing || difficulty == null) return;

        int newDistanceScore = Mathf.FloorToInt(difficulty.Distance * scorePerUnitDistance);
        if (newDistanceScore != distanceScore)
        {
            distanceScore = newDistanceScore;
            OnScoreChanged?.Invoke(Score);
        }
    }


    public void SetState(GameState newState)
    {
        if (newState == State) return;
        ApplyState(newState);
    }

    private void ApplyState(GameState newState)
    {
        State = newState;
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        OnStateChanged?.Invoke(newState);
    }

    private void HandlePlayerDied()
    {
        SetState(GameState.GameOver);
    }


    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (State == GameState.Playing) Pause();
        else if (State == GameState.Paused) Resume();
    }

    public void Pause()
    {
        if (State == GameState.Playing) SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (State == GameState.Paused) SetState(GameState.Playing);
    }


    public void AddScore(int amount)
    {
        if (State != GameState.Playing) return;
        bonusScore += amount;
        OnScoreChanged?.Invoke(Score);
    }


    [ContextMenu("Restart")]
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [ContextMenu("Go To Main Menu")]
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    // TODO
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 60, 400, 25), $"State: {State}   Score: {Score}");
    }
}