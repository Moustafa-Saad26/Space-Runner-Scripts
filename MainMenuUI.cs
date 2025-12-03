using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    
    public static MainMenuUI Instance {private set; get;}
    
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    
    [SerializeField] private Button musicButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private TextMeshProUGUI changeMusicText;
    [SerializeField] private TextMeshProUGUI changeSFXText;
    
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
        
        if(playButton == null) Debug.LogError("Play button not found");
        if(settingsButton == null) Debug.LogError("Settings button not found");
        if(musicButton == null) Debug.LogError("Music button not found");
        if(sfxButton == null) Debug.LogError("SFX button not found");
    }
    
    private void Start()
    {
        playButton.onClick.AddListener(PlayPressed);
        settingsButton.onClick.AddListener(SettingsPressed);
        musicButton.onClick.AddListener(MusicPressed);
        sfxButton.onClick.AddListener(SfxPressed);
        
        UpdateStartingVolume();
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeUpdate += SoundManager_OnVolumeUpdated;
        }
        
        musicButton.gameObject.SetActive(false);
        sfxButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(true);
    }
    
    private void SoundManager_OnVolumeUpdated(object sender, SoundManager.OnVolumeEventArgs e)
    {
        Debug.Log("Volume Updated");
        changeMusicText.text = "Music: " + e.musicVolume.ToString("#0");
        changeSFXText.text = "SFX: " + e.sfxVolume.ToString("#0");
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
    
    private void MusicPressed()
    {
        OnMusicChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void SfxPressed()
    {
        OnSFXChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void SettingsPressed()
    {
        musicButton.gameObject.SetActive(true);
        sfxButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(false);
    }

    private void PlayPressed()
    {
        Loader.LoadScene(Loader.Scene.GameScene);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(PlayPressed);
        settingsButton.onClick.RemoveListener(SettingsPressed);
        musicButton.onClick.RemoveListener(MusicPressed);
        sfxButton.onClick.RemoveListener(SfxPressed);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeUpdate -= SoundManager_OnVolumeUpdated;
        }
    }
}
