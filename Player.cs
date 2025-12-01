using UnityEngine;
using System;
using Unity.Cinemachine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player Instance {private set; get;}
    
    
    private float _moveSpeed;
    
    [SerializeField] private float changeLaneSpeed = 5f;
    
    private bool _canMove;
    private bool _isChangingLane;
    private int _currentLane; // 0 = Left, 1 = Middle, 2 = Right
    private readonly float[] _lanePositions = {8f, 0, -8f};
    private float _laneChangeTimer;
    private float _baseY;
    private bool _isLevitatingUp;
    private bool _isLevitating;
    private bool _isLevitatingDown;
    
    [SerializeField] private float driftSpeed = 0.6f;
    [SerializeField] private float driftRange = 10f;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private float floatDuration = 20f;
    private Vector3 _startPos;
    private float _seedX;
    private float _seedY;



    public event EventHandler OnGameOver;
    public event EventHandler OnSpawnTriggered;
    public event EventHandler OnDespawnTriggered;
    public event EventHandler<OnCoinCollectedEventArgs> OnCoinCollected;
    public event EventHandler<OnPowerUpCollectedEventArgs> OnPowerUpCollected;
    public event EventHandler OnPowerUpLevitatingDown;


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
        {
            GameInput.Instance.OnInputPressed += GameInput_OnInputPressed;
            GameInput.Instance.OnSwipeDown += GameInput_OnSwipeDown;
        }
        if (PowerUpsManager.Instance != null)
        {
            PowerUpsManager.Instance.OnPowerUpSpeedChangedActivated += PowerUpsManager_OnPowerUpSpeedChangedActivated;
            PowerUpsManager.Instance.OnPowerUpLevitationActivated += PowerUpsManager_OnPowerUpLevitationActivated;
            PowerUpsManager.Instance.OnPowerUpSlowMotionActivated += PowerUpsManager_OnPowerUpSlowMotionActivated;
        }

        if (SpeedManager.Instance != null)
        {
            SpeedManager.Instance.OnGameSpeedChanged += SpeedManager_OnGameSpeedChanged;
            _moveSpeed = SpeedManager.Instance.GetStartingSpeed();
        }
        
        _seedX = Random.value * 100f;
        _seedY = Random.value * 100f;
    }

    private void GameInput_OnSwipeDown(object sender, EventArgs e)
    {
        if (_isLevitating)
        {
            _isLevitatingDown = true;
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
        if(GameStates.currentGameState != GameStates.GameState.InGame) return;
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
        if (other.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.Explode();
            OnGameOver?.Invoke(this, EventArgs.Empty);
            StartFloating();
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
    
    private void StartFloating()
    {
        _startPos = transform.position;
        StartCoroutine(TransformFloatRoutine());
    }

    private System.Collections.IEnumerator TransformFloatRoutine()
    {
        float elapsed = 0f;
        while (true)
        {
            float t = elapsed * driftSpeed;
            float x = (Mathf.PerlinNoise(_seedX, t) - 0.5f) * 2f * driftRange;
            float y = (Mathf.PerlinNoise(_seedY, t) - 0.5f) * 2f * driftRange;
            transform.position = _startPos + new Vector3(x, y, 0f);

            transform.Rotate(Vector3.forward, rotateSpeed * Time.unscaledDeltaTime);

            elapsed += Time.unscaledDeltaTime; // use unscaled if you pause time
            yield return null;
        }

        // optional cleanup
    }

    private void MovePlayer()
    {
        if(GameStates.currentGameState != GameStates.GameState.InGame) return;
        transform.Translate(_moveSpeed * Time.deltaTime, 0, 0);
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
        float speedBeforeSlowMotion = _moveSpeed;
        float changeLaneSpeedBeforeSlowMotion = changeLaneSpeed;
        _moveSpeed /= (1/speed);
        changeLaneSpeed /= (1/speed);
        yield return new WaitForSeconds(slowMotionDuration);
        _moveSpeed = speedBeforeSlowMotion;
        changeLaneSpeed = changeLaneSpeedBeforeSlowMotion;
    }

    private System.Collections.IEnumerator Levitate(float levitateAmount, float timeToLevitate, float levitationDuration, float speed)
    {
        _isLevitatingUp = true;
        _isLevitating = false;
        _isLevitatingDown = false;
        Vector3 targetPosition = new Vector3(transform.position.x, _baseY + levitateAmount, transform.position.z);
        yield return StartCoroutine(LerpToTargetPosition(targetPosition, timeToLevitate, speed));
        _isLevitatingUp = false;
        _isLevitating = true;
        _isLevitatingDown = false;
        //
        //yield return new WaitForSeconds(levitationDuration);
        float elapsedTime = 0;
        while (elapsedTime < levitationDuration)
        {
            if(_isLevitatingDown) break;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //
        OnPowerUpLevitatingDown?.Invoke(this, EventArgs.Empty);
        targetPosition = new Vector3(transform.position.x, _baseY, transform.position.z);
        yield return StartCoroutine(LerpToTargetPosition(targetPosition, timeToLevitate, speed));
        _isLevitatingUp = false;
        _isLevitating = false;
        _isLevitatingDown = false;
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
        if(changeAmount > 0 && _moveSpeed >= maxSpeed) return;
        Debug.Log("Changing Speed from " + _moveSpeed + " to");
        _moveSpeed += changeAmount;
        Debug.Log(_moveSpeed);
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
            GameInput.Instance.OnSwipeDown -= GameInput_OnSwipeDown;
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
