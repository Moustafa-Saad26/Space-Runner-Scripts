using System;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsUI : MonoBehaviour
{
    [SerializeField] private RectTransform progressCircleRect;
    [SerializeField] private Image progressCircle;
    [SerializeField] private RectTransform powerUpHolderRect;
    
    private float _progress;
    private float _maxDuration;
    private float _progressCircleTimer;
    private float _timeToLevitateTimer;
    private float _timeToLevitate;
    
    private bool _isLevitating;
    private bool _isSlowMotion;
    
    
    private void Start()
    {
        if(progressCircleRect == null) Debug.LogError("Levitation Progress Rect is not set in PowerUpsUI");
        if(progressCircle == null) Debug.LogError("Levitation Progress Circle is not set in PowerUpsUI");
        if (PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpLevitationCollected += PowerUpsManager_OnLevitationCollected;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated += PowerUpsManager_OnLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpSlowMotionCollected += PowerUpsManager_OnSlowMotionCollected;
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated += PowerUpsManager_OnSlowMotionActivated;
            PowerUpsManager.Instance.OnPowerUpRocketCollected += PowerUpsManager_OnRocketCollected;
            PowerUpsManager.Instance.OnPowerUpRocketActivated += PowerUpsManager_OnRocketActivated;
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnPowerUpLevitatingDown += Player_OnLevitatingDown;
        }
        HideProgressCircle();
    }
    

    private void Update()
    {
        if (_isLevitating)
        {
            _timeToLevitateTimer += Time.deltaTime;
            if (_timeToLevitateTimer >= _timeToLevitate)
            {
                StartProgressCircle();
            }
        }
        
        if (_isSlowMotion)
        {
            StartProgressCircle();
        }
    }
    private void PowerUpsManager_OnRocketCollected(object sender, PowerUpsManager.OnRocketCollectedEventArgs e)
    {
        DestroyAnyPowerImage();
        Image rocketImage = Instantiate(e.rocketImage, powerUpHolderRect);
    }
    
    private void PowerUpsManager_OnRocketActivated(object sender, PowerUpsManager.OnRocketActivatedEventArgs e)
    {
        DestroyAnyPowerImage();
    }

    private void Player_OnLevitatingDown(object sender, EventArgs e)
    {
        _isLevitating = false;
        HideProgressCircle();
        ResetTimers();
        ResetBools();
    }

   

    private void PowerUpsManager_OnLevitationCollected(object sender, PowerUpsManager.OnLevitationCollectedEventArgs e)
    {
        DestroyAnyPowerImage();
        Image levitationImage = Instantiate(e.levitationImage, powerUpHolderRect);
    }
    
    private void PowerUpsManager_OnLevitationActivated(object sender, PowerUpsManager.OnLevitationActivatedEventArgs e)
    {
        _maxDuration = e.levitationDuration;
        _timeToLevitate = e.timeToLevitate;
        _isLevitating = true;
        DestroyAnyPowerImage();
    }
    
    
    
    private void PowerUpsManager_OnSlowMotionCollected(object sender, PowerUpsManager.OnSlowMotionCollectedEventArgs e)
    {
        DestroyAnyPowerImage();
        Image slowMoImage = Instantiate(e.slowMotionImage, powerUpHolderRect);
    }
    private void PowerUpsManager_OnSlowMotionActivated(object sender, PowerUpsManager.OnSlowMotionActivatedEventArgs e)
    {
        _maxDuration = e.slowMotionDuration;
        _isSlowMotion = true;
        DestroyAnyPowerImage();
    }

    
    
    

    private void DestroyAnyPowerImage()
    {
        if(powerUpHolderRect.childCount == 0) return;
        foreach (Transform child in powerUpHolderRect)
        {
            Destroy(child.gameObject);
        }
    }

    private void HideProgressCircle()
    {
        progressCircleRect.gameObject.SetActive(false);
    }

    private void ShowProgressCircle()
    {
        progressCircleRect.gameObject.SetActive(true);
    }

   

    private void ResetTimers()
    {
        _progress = 0;
        _timeToLevitateTimer = 0;
        _progressCircleTimer = 0;
    }
    
    private void ResetBools()
    {
        _isLevitating = false;
        _isSlowMotion = false;
    }

    private void StartProgressCircle()
    {
        ShowProgressCircle();
        _progressCircleTimer += Time.deltaTime;
        _progress = Mathf.Clamp01(_progressCircleTimer / _maxDuration);
        progressCircle.fillAmount = _progress;
        if (_progressCircleTimer >= _maxDuration)
        {
            HideProgressCircle();
            ResetTimers();
            ResetBools();
        }
    }
    private void OnDestroy()
    {
        if(PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpLevitationCollected -= PowerUpsManager_OnLevitationCollected;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated -= PowerUpsManager_OnLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpSlowMotionCollected -= PowerUpsManager_OnSlowMotionCollected;
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated -= PowerUpsManager_OnSlowMotionActivated;
            PowerUpsManager.Instance.OnPowerUpRocketCollected -= PowerUpsManager_OnRocketCollected;
            Player.Instance.OnPowerUpLevitatingDown -= Player_OnLevitatingDown;
            PowerUpsManager.Instance.OnPowerUpRocketActivated -= PowerUpsManager_OnRocketActivated;
        }
    }
}
