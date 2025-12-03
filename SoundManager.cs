using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    
    public static SoundManager Instance {private set; get;}
    
    [SerializeField] private AudioClip[] coinSounds;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip engineSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip soundtrack;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip slowMoSound;
    [SerializeField] private AudioClip levitateUpSound;
    [SerializeField] private AudioClip levitateDownSound;
    [SerializeField] private AudioClip rocketSound;
    
    
    
    private float _musicVolume;
    private float _sfxVolume;
    
    private SoundSource _musicSoundSource;
    private SoundSource _engineSoundSource;
    
    public event EventHandler<OnVolumeEventArgs> OnVolumeUpdate;

    public class OnVolumeEventArgs : EventArgs
    {
        public float sfxVolume;
        public float musicVolume;
    }
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _sfxVolume = PlayerPrefs.HasKey(GameStates.SFX_VOLUME) ? PlayerPrefs.GetFloat(GameStates.SFX_VOLUME) : 1f;
        _musicVolume = PlayerPrefs.HasKey(GameStates.MUSIC_VOLUME) ? PlayerPrefs.GetFloat(GameStates.MUSIC_VOLUME) : 1f;
        
        AudioListener.volume = 1f;
        
    }

    private void Start()
    {
        if(coinSounds.Length == 0) Debug.LogError("No coin sounds found");
        if(powerUpSound == null) Debug.LogError("No power up sound found");
        if(engineSound == null) Debug.LogError("No engine sound found");
        if(gameOverSound == null) Debug.LogError("No game over sound found");
        if(soundtrack == null) Debug.LogError("No soundtrack found");
        if(explosionSound == null) Debug.LogError("No Explosion sound found");
        if(slowMoSound == null) Debug.LogError("No Slow Motion sound found");
        if(levitateUpSound == null) Debug.LogError("No Levitate Up sound found");
        if(levitateDownSound == null) Debug.LogError("No Levitate Down sound found");
        

        if (Player.Instance != null)
        {
            Player.Instance.OnCoinCollected += Player_OnCoinCollected;
            Player.Instance.OnPowerUpCollected += Player_OnPowerUpCollected;
            Player.Instance.OnGameOver += Player_OnGameOver;
            Player.Instance.OnPowerUpLevitatingDown += Player_OnLevitatingDown;
        }

        if (PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated += PowerUpsManager_OnSlowMotionActivated;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated += PowerUpsManager_OnPowerUpLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpRocketActivated += PowerUpsManager_OnRocketActivated;
        }

        if (PausedUI.Instance != null)
        {
            PausedUI.Instance.OnPause += PausedUI_OnPause;
            PausedUI.Instance.OnResume += PausedUI_OnResume;
            PausedUI.Instance.OnMusicChanged += Settings_OnMusicChanged;
            PausedUI.Instance.OnSFXChanged += Settings_OnSFXChanged;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus += GameStateManager_OnGameLostFocus;
            GameStateManager.Instance.OnGameRegainedFocus += GameStateManager_OnGameRegainedFocus;
        }

        if (MainMenuUI.Instance != null)
        {
            MainMenuUI.Instance.OnMusicChanged += Settings_OnMusicChanged;
            MainMenuUI.Instance.OnSFXChanged += Settings_OnSFXChanged;
        }
        
        Obstacle.OnObstacleExploded += Obstacle_OnObstacleExploded;
        if (SceneManager.GetActiveScene().name == nameof(Loader.Scene.GameScene))
        {
            PlayEngineSound();
        }
        PlaySoundtrack();
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void Obstacle_OnObstacleExploded(object sender, EventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlaySound2D(explosionSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 2f);
    }

    private void PowerUpsManager_OnRocketActivated(object sender, PowerUpsManager.OnRocketActivatedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlaySound2D(rocketSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, e.rocketDuration + 2f);
    }

    private void Player_OnLevitatingDown(object sender, EventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlayLongSound2D(levitateDownSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource,  3f);
    }


    private void Settings_OnSFXChanged(object sender, EventArgs e)
    {
        ChangeSFXVolume();
    }

    private void Settings_OnMusicChanged(object sender, EventArgs e)
    {
        ChangeMusicVolume();
    }

    private void PausedUI_OnResume(object sender, EventArgs e)
    {
        AudioListener.volume = 1f;
    }

    private void GameStateManager_OnGameRegainedFocus(object sender, EventArgs e)
    {
        AudioListener.pause = false;
    }

    private void GameStateManager_OnGameLostFocus(object sender, EventArgs e)
    {
        AudioListener.pause = true;
    }

    private void PausedUI_OnPause(object sender, EventArgs e)
    {
        AudioListener.volume = 0.5f;
    }

    private void PowerUpsManager_OnPowerUpLevitationActivated(object sender, PowerUpsManager.OnLevitationActivatedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlayLongSound2D(levitateUpSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, e.levitationDuration + 2f);
    }

    

    private void PowerUpsManager_OnSlowMotionActivated(object sender, PowerUpsManager.OnSlowMotionActivatedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlayLongSound2D(slowMoSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, e.slowMotionDuration + 2f);
    }

    private void Player_OnGameOver(object sender, EventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlaySound2D(gameOverSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 2f);
    }

    private void Player_OnPowerUpCollected(object sender, Player.OnPowerUpCollectedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        if(e.powerUpType == PowerUp.PowerUpType.IncreaseSpeed) audioSource.SetPitch(1f);
        if(e.powerUpType == PowerUp.PowerUpType.DecreaseSpeed) audioSource.SetPitch(0.6f);
        if(e.powerUpType == PowerUp.PowerUpType.SlowMotion) audioSource.SetPitch(0.8f);
        audioSource.PlaySound2D(powerUpSound, _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 1.5f);
    }

    private void Player_OnCoinCollected(object sender, Player.OnCoinCollectedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.SetPitch(0.7f);
        audioSource.PlaySound2D(coinSounds[0], _sfxVolume);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 1.5f);
        //PlayRandomClip(coinSounds);
    }

    

    private void PlayEngineSound()
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        _engineSoundSource = audioSource;
        audioSource.PlaySound2DLooped(engineSound, _sfxVolume);
    }

   

    private void PlaySoundtrack()
    {
        if (AudioPoolManager.Instance == null)
        {
            SoundSource soundSource = gameObject.AddComponent<SoundSource>();
            _musicSoundSource = soundSource;
            soundSource.PlaySound2DLooped(soundtrack, _musicVolume);
            return;
        }
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        _musicSoundSource = audioSource;
        audioSource.PlaySound2DLooped(soundtrack, _musicVolume);
    }
    
    private void ChangeSFXVolume()
    {
        _sfxVolume += 0.1f;

        if (_sfxVolume > 1f)
        {
            _sfxVolume = 0f;
        }
        _engineSoundSource?.changeVolume(_sfxVolume);
        OnVolumeUpdate?.Invoke(this, new OnVolumeEventArgs {musicVolume = _musicVolume * 10, sfxVolume = _sfxVolume * 10});
        
        PlayerPrefs.SetFloat(GameStates.SFX_VOLUME, _sfxVolume);
        PlayerPrefs.Save();
    }
    
    private void ChangeMusicVolume()
    {
        _musicVolume += 0.1f;

        if (_musicVolume > 1f)
        {
            _musicVolume = 0f;
        }
        _musicSoundSource.changeVolume(_musicVolume);
        OnVolumeUpdate?.Invoke(this, new OnVolumeEventArgs {musicVolume = _musicVolume * 10, sfxVolume = _sfxVolume * 10});

        PlayerPrefs.SetFloat(GameStates.MUSIC_VOLUME, _musicVolume);
        PlayerPrefs.Save();
    }
    
    
    private void OnDestroy()
    {
        if(Player.Instance != null)
        {
            Player.Instance.OnCoinCollected -= Player_OnCoinCollected;
            Player.Instance.OnPowerUpCollected -= Player_OnPowerUpCollected;
            Player.Instance.OnGameOver -= Player_OnGameOver;
        }
        if(PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated -= PowerUpsManager_OnSlowMotionActivated;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated -= PowerUpsManager_OnPowerUpLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpRocketActivated -= PowerUpsManager_OnRocketActivated;
        }
        if(PausedUI.Instance != null)
        {
            PausedUI.Instance.OnPause -= PausedUI_OnPause;
            PausedUI.Instance.OnResume -= PausedUI_OnResume;
            PausedUI.Instance.OnMusicChanged -= Settings_OnMusicChanged;
            PausedUI.Instance.OnSFXChanged -= Settings_OnSFXChanged;
        }
        if(GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus -= GameStateManager_OnGameLostFocus;
            GameStateManager.Instance.OnGameRegainedFocus -= GameStateManager_OnGameRegainedFocus;
        }

        if (MainMenuUI.Instance != null)
        {
            MainMenuUI.Instance.OnMusicChanged -= Settings_OnMusicChanged;
            MainMenuUI.Instance.OnSFXChanged -= Settings_OnSFXChanged;
        }
        
        Obstacle.OnObstacleExploded -= Obstacle_OnObstacleExploded;
    }
}
