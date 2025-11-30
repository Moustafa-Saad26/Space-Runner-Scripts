using System;
using UnityEngine;
using UnityEngine.UI;

public class PausedUI : MonoBehaviour
{
    public static PausedUI Instance {private set; get;}
    
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button pauseButton;

    public event EventHandler onPause;
    public event EventHandler onResume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if(pauseMenu == null) Debug.LogError("Pause menu not found");
        if(resumeButton == null) Debug.LogError("Resume button not found");
        if(settingsButton == null) Debug.LogError("Settings button not found");
        if(restartButton == null) Debug.LogError("Restart button not found");
        if(pauseButton == null) Debug.LogError("Pause button not found");
    }
    
    private void Start()
    {
        pauseMenu.SetActive(false);
        restartButton.onClick.AddListener(RestartPressed);
        pauseButton.onClick.AddListener(PausePressed);
        resumeButton.onClick.AddListener(ResumePressed);
        if (Player.Instance != null)
        {
            Player.Instance.OnGameOver += Player_OnGameOver;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus += GameStateManager_OnGameLostFocus;
        }
    }

    private void GameStateManager_OnGameLostFocus(object sender, EventArgs e)
    {
        PausePressed();
    }

    private void PausePressed()
    {
        pauseMenu.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        Time.timeScale = 0;
        onPause?.Invoke(this, EventArgs.Empty);
    }
    
    private void ResumePressed()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        onResume?.Invoke(this, EventArgs.Empty);
    }
    private void Player_OnGameOver(object sender, EventArgs e)
    {
        pauseMenu.SetActive(true);
        resumeButton.gameObject.SetActive(false);
    }

    private void RestartPressed()
    {
        Loader.LoadScene(Loader.Scene.GameScene);
    }

    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(ResumePressed);
        pauseButton.onClick.RemoveListener(PausePressed);
        restartButton.onClick.RemoveListener(RestartPressed);
        if (Player.Instance != null)
        {
            Player.Instance.OnGameOver -= Player_OnGameOver;
        }
    }
    
    
}
