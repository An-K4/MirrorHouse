using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GAME MANAGER - Quản lý vòng lặp chơi game (Game Loop), các trạng thái game và chuyển cảnh
/// </summary>
public class MirrorHouseGameManager : MonoBehaviour
{
    public static MirrorHouseGameManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string mainGameSceneName = "Level_Main";

    private float gameStartTime;
    private float finalElapsedTime;

    public GameState CurrentState => currentState;
    public float ElapsedTime => currentState == GameState.Playing ? (Time.time - gameStartTime) : finalElapsedTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;
        gameStartTime = Time.time;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerCaught += HandlePlayerCaught;
        GameEvents.OnGameWin += HandleGameWin;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerCaught -= HandlePlayerCaught;
        GameEvents.OnGameWin -= HandleGameWin;
    }

    private void Update()
    {
        // PC Pause Toggle via Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Paused;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowPauseMenu(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;

        currentState = GameState.Playing;
        Time.timeScale = 1f;

        #if !UNITY_ANDROID && !UNITY_IOS
        Cursor.lockState = CursorLockMode.None;
        #endif

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowPauseMenu(false);
            GameUIManager.Instance.ShowSettingsMenu(false);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            // Fallback reload current scene if MainMenu scene not yet created in build settings
            Debug.LogWarning($"[GameManager] Scene '{mainMenuSceneName}' chưa có trong build settings. Tải lại scene hiện tại.");
            RestartLevel();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded(mainGameSceneName))
        {
            SceneManager.LoadScene(mainGameSceneName);
        }
        else
        {
            SceneManager.LoadScene("Level_Main");
        }
    }

    public void QuitGame()
    {
        Debug.Log("[GameManager] Thoát game...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void HandlePlayerCaught()
    {
        if (currentState == GameState.GameOver || currentState == GameState.Victory) return;

        currentState = GameState.GameOver;
        finalElapsedTime = Time.time - gameStartTime;
        Debug.Log($"[GameManager] GAME OVER! Người chơi bị bắt sau {finalElapsedTime:F1}s.");

        // Show Game Over UI after a short jumpscare pause
        Invoke(nameof(ShowGameOverUI), 1.2f);
    }

    private void ShowGameOverUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowGameOver(true);
        }
    }

    private void HandleGameWin()
    {
        if (currentState == GameState.GameOver || currentState == GameState.Victory) return;

        currentState = GameState.Victory;
        finalElapsedTime = Time.time - gameStartTime;
        Debug.Log($"[GameManager] VICTORY! Người chơi đã thoát trong {finalElapsedTime:F1}s!");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowVictory(true, finalElapsedTime);
        }
    }
}
