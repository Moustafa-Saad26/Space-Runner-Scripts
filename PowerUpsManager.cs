using System;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsManager : MonoBehaviour
{
    public static PowerUpsManager Instance {private set; get;}
    
    
    
    [Header("SlowMotion Configuration")]
    [SerializeField] private float slowMotionDuration = 2f;
    [SerializeField][Range(0,1)] private float slowMotionSpeed = 0.5f;
    [SerializeField] private Image slowMotionImage;
    
    [Header("Levitation Configuration")]
    [SerializeField] private Image levitationImage;
    [SerializeField] private float levitationDuration = 3f;
    [SerializeField] private float timeToLevitate = 1f;
    [SerializeField] private float levitationAmount = 5f;
    [SerializeField] private float levitationSpeed = 1f;
    private float _levitationTotalDuration;
    
    [Header("Rocket Configuration")]
    [SerializeField] private Image rocketImage;
    [SerializeField] private Rocket rocketPrefab;
    [SerializeField] private float rocketSpeed = 10f;
    [SerializeField] private float rocketDuration = 2f;
    
    [Header("Increase/Decrease Speed Configuration")]
    [SerializeField] private float powerUpIncreaseSpeedAmount = 2f;
    [SerializeField] private float powerUpDecreaseSpeedAmount = 2f;
    
    public event EventHandler<OnSpeedChangedEventArgs> OnPowerUpSpeedChangedActivated;
    
    public event EventHandler<OnLevitationCollectedEventArgs> OnPowerUpLevitationCollected;
    public event EventHandler<OnLevitationActivatedEventArgs> OnPowerUpLevitationActivated;
    
    public event EventHandler<OnSlowMotionCollectedEventArgs> OnPowerUpSlowMotionCollected;
    public event EventHandler<OnSlowMotionActivatedEventArgs> OnPowerUpSlowMotionActivated;
    
    public event EventHandler<OnRocketCollectedEventArgs> OnPowerUpRocketCollected;
    public event EventHandler<OnRocketActivatedEventArgs> OnPowerUpRocketActivated;
    
    private PowerUp.PowerUpType _currentPowerUp;
    private int _levitationCount;
    private float _activePowerUpCooldown;
    private bool _isPowerUpActive;

    public class OnRocketActivatedEventArgs : EventArgs
    {
        public Rocket rocket;
        public float rocketSpeed;
        public float rocketDuration;
    }
    
    public class OnRocketCollectedEventArgs : EventArgs
    {
        public Image rocketImage;
    }
    
    public class OnSpeedChangedEventArgs : EventArgs
    {
        public float speedIncreaseAmount;
    }
    public class OnSlowMotionActivatedEventArgs : EventArgs
    {
        public float slowMotionDuration;
        public float slowMotionSpeed;
    }
    
    public class OnLevitationActivatedEventArgs : EventArgs
    {
        public float levitationAmount;
        public float levitationSpeed;
        public float levitationDuration;
        public float timeToLevitate;
    }
    
    public class OnLevitationCollectedEventArgs : EventArgs
    {
        public Image levitationImage;
    }

    public class OnSlowMotionCollectedEventArgs : EventArgs
    {
        public Image slowMotionImage;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if(Player.Instance != null)
            Player.Instance.OnPowerUpCollected += Player_OnPowerUpCollected;
        if(GameInput.Instance != null)
            GameInput.Instance.OnSwipeUp += GameInputOnSwipeUp;
        if (levitationImage == null) Debug.LogError("Levitation Image is not set in DifficultyManager");
        if (slowMotionImage == null) Debug.LogError("Slow Motion Image is not set in DifficultyManager");
        if (rocketImage == null) Debug.LogError("Rocket Image is not set in DifficultyManager");
        _levitationTotalDuration = levitationDuration + (2 * timeToLevitate);
    }
    
    private void Update()
    {
        if (_isPowerUpActive)
        {
            _activePowerUpCooldown -= Time.deltaTime;
            if(_activePowerUpCooldown <= 0) _isPowerUpActive = false;
        }
    }
    
    
    private void GameInputOnSwipeUp(object sender, EventArgs e)
    {
        if(GameStates.currentGameState != GameStates.GameState.InGame) return;
        if(_isPowerUpActive) return;
        switch (_currentPowerUp)
        {
            case PowerUp.PowerUpType.None:
                //Debug.LogError("No PowerUp Active");
                break;
            case PowerUp.PowerUpType.SlowMotion:
                PowerUpSlowMotion();
                _isPowerUpActive = true;
                _activePowerUpCooldown = slowMotionDuration;
                _currentPowerUp = PowerUp.PowerUpType.None;
                break;
            case PowerUp.PowerUpType.Levitation:
                PowerUpLevitation();
                _isPowerUpActive = true;
                _activePowerUpCooldown = _levitationTotalDuration;
                _currentPowerUp = PowerUp.PowerUpType.None;
                break;
            case PowerUp.PowerUpType.Rocket:
                PowerUpRocket();
                _currentPowerUp = PowerUp.PowerUpType.None;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Player_OnPowerUpCollected(object sender, Player.OnPowerUpCollectedEventArgs e)
    {
        switch (e.powerUpType)
        {
            case PowerUp.PowerUpType.IncreaseSpeed:
                PowerUpIncreaseSpeed();
                break;
            case PowerUp.PowerUpType.DecreaseSpeed:
                PowerUpDecreaseSpeed();
                break;
            case PowerUp.PowerUpType.SlowMotion:
                _currentPowerUp = PowerUp.PowerUpType.SlowMotion;
                OnPowerUpSlowMotionCollected?.Invoke(this, new OnSlowMotionCollectedEventArgs
                {
                    slowMotionImage = slowMotionImage
                });
                break;
            case PowerUp.PowerUpType.Levitation:
                _currentPowerUp = PowerUp.PowerUpType.Levitation;
                OnPowerUpLevitationCollected?.Invoke(this, new OnLevitationCollectedEventArgs
                {
                    levitationImage = levitationImage
                });
                break;
            case PowerUp.PowerUpType.Shield:
                PowerUpShield();
                break;
            case PowerUp.PowerUpType.Rocket:
                _currentPowerUp = PowerUp.PowerUpType.Rocket;
                OnPowerUpRocketCollected?.Invoke(this, new OnRocketCollectedEventArgs
                {
                    rocketImage = rocketImage
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

   
    

    private void PowerUpIncreaseSpeed()
    {
      
        OnPowerUpSpeedChangedActivated?.Invoke(this, new OnSpeedChangedEventArgs
        {
            speedIncreaseAmount = powerUpIncreaseSpeedAmount,
        });
    }

    private void PowerUpDecreaseSpeed()
    {
        OnPowerUpSpeedChangedActivated?.Invoke(this, new OnSpeedChangedEventArgs
        {
            speedIncreaseAmount = -powerUpDecreaseSpeedAmount,
        });
    }

    private void PowerUpSlowMotion()
    {
        OnPowerUpSlowMotionActivated?.Invoke(this, new OnSlowMotionActivatedEventArgs
        {
            slowMotionDuration = slowMotionDuration,
            slowMotionSpeed = slowMotionSpeed
            
        });
    }
    
    private void PowerUpLevitation()
    {
        OnPowerUpLevitationActivated?.Invoke(this, new OnLevitationActivatedEventArgs
        {
            levitationAmount = levitationAmount,
            levitationSpeed = levitationSpeed,
            levitationDuration = levitationDuration,
            timeToLevitate = timeToLevitate
        });
    }
    
    private void PowerUpRocket()
    {
        OnPowerUpRocketActivated?.Invoke(this, new OnRocketActivatedEventArgs
        {
            rocket = rocketPrefab,
            rocketSpeed = rocketSpeed,
            rocketDuration = rocketDuration
        });
    }

    private void PowerUpShield()
    {
        
    }
    
    private void OnDestroy()
    {
        if(Player.Instance != null)
            Player.Instance.OnPowerUpCollected -= Player_OnPowerUpCollected;
    }
}
