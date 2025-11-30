using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance {private set; get;}
    
    [Header("Starting Speed")]
    [SerializeField] private float moveSpeed = 3f;
    
    [SerializeField] private float changeLaneSpeed = 5f;
    
    private bool _canMove;
    private bool _isChangingLane;
    private int _currentLane; // 0 = Left, 1 = Middle, 2 = Right
    private readonly float[] _lanePositions = {8f, 0, -8f};
    private float _laneChangeTimer;
    private float _baseY;

    public event EventHandler OnGameOver;
    public event EventHandler OnSpawnTriggered;
    public event EventHandler OnDespawnTriggered;
    public event EventHandler<OnCoinCollectedEventArgs> OnCoinCollected;
    public event EventHandler<OnPowerUpCollectedEventArgs> OnPowerUpCollected;

    public class OnPowerUpCollectedEventArgs : EventArgs
    {
        public PowerUp.PowerUpType powerUpType;
    }
    
    public class OnCoinCollectedEventArgs : EventArgs
    {
        public int coinValue;
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

    void Start()
    {
        Time.timeScale = 1;
        _baseY = transform.position.y;
        _canMove = true;
        _currentLane = 1; // Start in MiddleLane
        if(GameInput.Instance != null)
            GameInput.Instance.OnInputPressed += GameInput_OnInputPressed;
        if (PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSpeedChangedActivated += PowerUpsManager_OnPowerUpSpeedChangedActivated;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated += PowerUpsManager_OnPowerUpLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated += PowerUpsManager_OnPowerUpSlowMotionActivated;
        }

        if (SpeedManager.Instance != null)
        {
            SpeedManager.Instance.OnGameSpeedChanged += SpeedManager_OnGameSpeedChanged;
        }
    }

    private void SpeedManager_OnGameSpeedChanged(object sender, SpeedManager.OnGameSpeedChangedEventArgs e)
    {
        ChangeSpeed(e.speedIncreaseAmount);
    }

    private void PowerUpsManager_OnPowerUpSlowMotionActivated(object sender, PowerUpsManager.OnSlowMotionActivatedEventArgs e)
    {
        StartCoroutine(SlowMotion(e.slowMotionDuration, e.slowMotionSpeed));
    }

    private void PowerUpsManager_OnPowerUpLevitationActivated(object sender, PowerUpsManager.OnLevitationActivatedEventArgs e)
    {
        StartCoroutine(Levitate(e.levitationAmount, e.timeToLevitate, e.levitationDuration, e.levitationSpeed));
    }

    private void PowerUpsManager_OnPowerUpSpeedChangedActivated(object sender, PowerUpsManager.OnSpeedChangedEventArgs e)
    {
        ChangeSpeed(e.speedIncreaseAmount);
    }

    private void GameInput_OnInputPressed(object sender, GameInput.OnInputPressedEventArgs e)
    {
        
        if (e.moveDirection == Vector3.right)
        {
            MoveRight();
        }

        if (e.moveDirection == Vector3.left)
        {
            MoveLeft();
        }
        
    }


    private void Update()
    {
        if (_canMove)
        {
            MovePlayer();
            if (_isChangingLane) ChangeLane(_currentLane);
        }
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>())
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
            StopPlayer();
        }

        if (other.GetComponent<SpawnTrigger>())
        {
            //Debug.Log("Spawn Trigger");
            OnSpawnTriggered?.Invoke(this, EventArgs.Empty);
        }

        if (other.GetComponent<DeSpawnTrigger>())
        {
            //Debug.Log("Despawn Trigger");
            OnDespawnTriggered?.Invoke(this, EventArgs.Empty);
        }

        if (other.TryGetComponent(out Coin coin))
        {
            //Debug.Log("Coin Collected");
            OnCoinCollected?.Invoke(this, new OnCoinCollectedEventArgs
            {
                coinValue = coin.GetCoinValue()
            });
            coin.CollectItem();
        }

        if (other.TryGetComponent(out PowerUp powerUp))
        {
            Debug.Log("PowerUp Collected: " + powerUp.name);
            OnPowerUpCollected?.Invoke(this, new OnPowerUpCollectedEventArgs
            {
                powerUpType = powerUp.GetPowerUpType()
            });
            powerUp.CollectItem();
        }
    }
    private void MovePlayer()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
    }
    
    private void MoveRight()
    {
        //Debug.Log("Move Right");
        if (_currentLane >= 2) return;
        _currentLane++;
        _isChangingLane = true; // Trigger for Update
    }

    private void MoveLeft()
    {
        //Debug.Log("Move Left");
        if (_currentLane <= 0) return;
        _currentLane--;
        _isChangingLane = true; // Trigger for Update
    }
    
    private void ChangeLane(int newLane)
    {
        float newZPosition = Mathf.Lerp(transform.position.z, _lanePositions[newLane], Time.deltaTime * changeLaneSpeed);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

        if (Mathf.Abs(transform.position.z - _lanePositions[newLane]) < 0.05f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _lanePositions[newLane]);
            _isChangingLane = false; // To disable the change lane in update for performance
        }
            
    }

    private System.Collections.IEnumerator SlowMotion(float slowMotionDuration, float speed)
    {
        Debug.Log("Slow Motion");
        float speedBeforeSlowMotion = moveSpeed;
        float changeLaneSpeedBeforeSlowMotion = changeLaneSpeed;
        moveSpeed /= (1/speed);
        changeLaneSpeed /= (1/speed);
        yield return new WaitForSeconds(slowMotionDuration);
        moveSpeed = speedBeforeSlowMotion;
        changeLaneSpeed = changeLaneSpeedBeforeSlowMotion;
    }

    private System.Collections.IEnumerator Levitate(float levitateAmount, float timeToLevitate, float levitationDuration, float speed)
    {
        Debug.Log("Levitating");
        Vector3 targetPosition = new Vector3(transform.position.x, _baseY + levitateAmount, transform.position.z);
        yield return StartCoroutine(LerpToTargetPosition(targetPosition, timeToLevitate, speed));
        yield return new WaitForSeconds(levitationDuration);
        targetPosition = new Vector3(transform.position.x, _baseY, transform.position.z);
        yield return StartCoroutine(LerpToTargetPosition(targetPosition, timeToLevitate, speed));
        
    }
    

    private System.Collections.IEnumerator LerpToTargetPosition(Vector3 targetPosition, float timeToLevitate, float speed)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < timeToLevitate)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01((elapsedTime / timeToLevitate) * speed);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        transform.position = targetPosition;
    }
    
    private void ChangeSpeed(float changeAmount)
    {
        float maxSpeed;
        if (SpeedManager.Instance == null)
        {
            Debug.LogError("SpeedManager Instance is null in Player");
            maxSpeed = 40;
        }
        else
            maxSpeed = SpeedManager.Instance.GetMaxPlayerSpeed();
        if(changeAmount > 0 && moveSpeed >= maxSpeed) return;
        Debug.Log("Changing Speed from " + moveSpeed + " to");
        moveSpeed += changeAmount;
        Debug.Log(moveSpeed);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    private void StopPlayer()
    {
        _canMove = false;
    }
    
    private void OnDestroy()
    {
        if(GameInput.Instance != null)
        {
            GameInput.Instance.OnInputPressed -= GameInput_OnInputPressed;
        }
        if(PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSpeedChangedActivated -= PowerUpsManager_OnPowerUpSpeedChangedActivated;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated -= PowerUpsManager_OnPowerUpLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated -= PowerUpsManager_OnPowerUpSlowMotionActivated;
        }
        if (SpeedManager.Instance != null)
        {
            SpeedManager.Instance.OnGameSpeedChanged -= SpeedManager_OnGameSpeedChanged;
        }
    }
}
