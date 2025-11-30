using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    
    
    [SerializeField] private AudioClip[] coinSounds;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip engineSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip soundtrack;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip slowMoSound;
    [SerializeField] private AudioClip levitateUpSound;
    [SerializeField] private AudioClip levitateDownSound;
    
    
    private void Awake()
    {
       
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
        }

        if (PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated += PowerUpsManager_OnSlowMotionActivated;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated += PowerUpsManager_OnPowerUpLevitationActivated;
        }

        if (PausedUI.Instance != null)
        {
            PausedUI.Instance.onPause += PausedUI_OnPause;
            PausedUI.Instance.onResume += PausedUI_OnResume;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus += GameStateManager_OnGameLostFocus;
            GameStateManager.Instance.OnGameRegainedFocus += GameStateManager_OnGameRegainedFocus;
        }
        PlayEngineSound();
        PlaySoundtrack();
        AudioListener.pause = false;
        AudioListener.volume = 1f;
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
       StartCoroutine(PlayLevitation(e.levitationDuration));
    }

    private System.Collections.IEnumerator PlayLevitation(float levitationDuration)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlayLongSound2D(levitateUpSound);
        yield return new WaitForSeconds(levitationDuration);
        audioSource.PlayLongSound2D(levitateDownSound);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 4f);
    }

    private void PowerUpsManager_OnSlowMotionActivated(object sender, PowerUpsManager.OnSlowMotionActivatedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlayLongSound2D(slowMoSound);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, e.slowMotionDuration + 2f);
    }

    private void Player_OnGameOver(object sender, EventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlaySound2D(gameOverSound);
        
        SoundSource audioSource2 = AudioPoolManager.Instance.GetObject();
        audioSource2.transform.SetParent(transform);
        audioSource2.PlaySound2D(explosionSound);
        
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 2f);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource2, 2f);
    }

    private void Player_OnPowerUpCollected(object sender, Player.OnPowerUpCollectedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        if(e.powerUpType == PowerUp.PowerUpType.IncreaseSpeed) audioSource.SetPitch(1f);
        if(e.powerUpType == PowerUp.PowerUpType.DecreaseSpeed) audioSource.SetPitch(0.6f);
        if(e.powerUpType == PowerUp.PowerUpType.SlowMotion) audioSource.SetPitch(0.8f);
        audioSource.PlaySound2D(powerUpSound);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 1.5f);
    }

    private void Player_OnCoinCollected(object sender, Player.OnCoinCollectedEventArgs e)
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.SetPitch(0.7f);
        audioSource.PlaySound2D(coinSounds[0]);
        AudioPoolManager.Instance.ReturnObjectAfter(audioSource, 1.5f);
        //PlayRandomClip(coinSounds);
    }

    

    private void PlayEngineSound()
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlaySound2DLooped(engineSound);
    }

   

    private void PlaySoundtrack()
    {
        SoundSource audioSource = AudioPoolManager.Instance.GetObject();
        audioSource.transform.SetParent(transform);
        audioSource.PlaySound2DLooped(soundtrack);
    }
    
    
    private void OnDestroy()
    {
        if(Player.Instance != null)
        {
            Player.Instance.OnCoinCollected -= Player_OnCoinCollected;
            Player.Instance.OnPowerUpCollected -= Player_OnPowerUpCollected;
        }
        if(PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated -= PowerUpsManager_OnSlowMotionActivated;
        }
        if(PausedUI.Instance != null)
        {
            PausedUI.Instance.onPause -= PausedUI_OnPause;
            PausedUI.Instance.onResume -= PausedUI_OnResume;
        }
        if(GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameLostFocus -= GameStateManager_OnGameLostFocus;
            GameStateManager.Instance.OnGameRegainedFocus -= GameStateManager_OnGameRegainedFocus;
        }
    }
}
