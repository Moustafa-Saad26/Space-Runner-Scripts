using System;
using TMPro;
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
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Button settingsResumeButton;
    [SerializeField] private Button settingsRestartButton;
    [SerializeField] private Button changeMusicButton;
    [SerializeField] private TextMeshProUGUI changeMusicText;
    [SerializeField] private Button changeSFXButton;
    [SerializeField] private TextMeshProUGUI changeSFXText;
    [SerializeField] private GameObject tutorialMenu;

    public event EventHandler OnPause;
    public event EventHandler OnResume;
    public event EventHandler OnMusicChanged;
    public event EventHandler OnSFXChanged;

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
        if(settingsResumeButton == null) Debug.LogError("Settings Resume button not found");
        if(settingsRestartButton == null) Debug.LogError("Settings Restart button not found");
        if(changeMusicButton == null) Debug.LogError("Change Music button not found");
        if(changeMusicText == null) Debug.LogError("Change Music Text not found");
        if(changeSFXButton == null) Debug.LogError("Change SFX button not found");
        if(changeSFXText == null) Debug.LogError("Change SFX Text not found");
        if(tutorialMenu == null) Debug.LogError("Tutorial Menu not found");
        
        

    }
    
    private void Start()
    {
        tutorialMenu.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        restartButton.onClick.AddListener(RestartPressed);
        pauseButton.onClick.AddListener(PausePressed);
        resumeButton.onClick.AddListener(ResumePressed);
        settingsButton.onClick.AddListener(SettingsPressed);
        settingsResumeButton.onClick.AddListener(ResumePressed);
        settingsRestartButton.onClick.AddListener(RestartPressed);
        changeMusicButton.onClick.AddListener(ChangeMusicVolumePressed);
        changeSFXButton.onClick.AddListener(ChangeSFXVolumePressed);
        if (Player.Instance != null)
        {
            Player.Instance.OnGameOver += Player_OnGameOver;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus += GameStateManager_OnGameLostFocus;
        }

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnSwipeUp += GameInput_OnSwipeUp;
        }
    
            
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeUpdate += SoundManager_OnVolumeUpdated;
        }
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeUpdate += SoundManager_OnVolumeUpdated;
        }
        
        UpdateStartingVolume();
    }
    

    private void GameInput_OnSwipeUp(object sender, EventArgs e)
    {
        GameStates.currentGameState = GameStates.GameState.InGame;
        tutorialMenu.SetActive(false);
    }


    private void UpdateStartingVolume()
    {
        float sfxVolume = 1;
        float musicVolume = 1;
        if (PlayerPrefs.HasKey(GameStates.SFX_VOLUME)) sfxVolume = PlayerPrefs.GetFloat(GameStates.SFX_VOLUME, 1f);
        if(PlayerPrefs.HasKey(GameStates.MUSIC_VOLUME)) musicVolume = PlayerPrefs.GetFloat(GameStates.MUSIC_VOLUME, 1f);
        sfxVolume *= 10;
        musicVolume *= 10;
        changeMusicText.text = "Music: " + musicVolume.ToString("#0");
        changeSFXText.text = "SFX: " + sfxVolume.ToString("#0");

    }
    
    
    private void SoundManager_OnVolumeUpdated(object sender, SoundManager.OnVolumeEventArgs e)
    {
        Debug.Log("Volume Updated");
        changeMusicText.text = "Music: " + e.musicVolume.ToString("#0");
        changeSFXText.text = "SFX: " + e.sfxVolume.ToString("#0");
    }

    

    private void ChangeMusicVolumePressed()
    {
        OnMusicChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void ChangeSFXVolumePressed()
    {
        OnSFXChanged?.Invoke(this, EventArgs.Empty);
    }
    

    private void SettingsPressed()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    private void GameStateManager_OnGameLostFocus(object sender, EventArgs e)
    {
        PausePressed();
    }

    private void PausePressed()
    {
        GameStates.currentGameState = GameStates.GameState.Paused;
        pauseButton.gameObject.SetActive(false);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        Time.timeScale = 0;
        OnPause?.Invoke(this, EventArgs.Empty);
    }
    
    private void ResumePressed()
    {
        GameStates.currentGameState = GameStates.GameState.InGame;
        pauseButton.gameObject.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1;
        OnResume?.Invoke(this, EventArgs.Empty);
    }
    private void Player_OnGameOver(object sender, EventArgs e)
    {
        GameStates.currentGameState = GameStates.GameState.GameOver; 
        StartCoroutine(DelayedGameOverUI(2.5f));
        
    }

    private System.Collections.IEnumerator DelayedGameOverUI(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus -= GameStateManager_OnGameLostFocus;
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeUpdate -= SoundManager_OnVolumeUpdated;
        }
    }
    
    
}
