using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    //Public Functions//
    // GetScore
    public static ScoreManager Instance {private set; get;}
    
    [SerializeField] private TextMeshProUGUI scoreText;
    private int _score;
    private int _highScore;


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
        if(scoreText == null) Debug.LogError("ScoreText is not set in ScoreManager");
        if(Player.Instance != null)
        {
            Player.Instance.OnCoinCollected += Player_OnCoinCollected;
            Player.Instance.OnGameOver += Player_OnGameOver;
        }
        
        _highScore = PlayerPrefs.HasKey(GameStates.HIGH_SCORE) ? PlayerPrefs.GetInt(GameStates.HIGH_SCORE) : 0;
        
        UpdateScoreText();
    }

    private void Player_OnGameOver(object sender, EventArgs e)
    {
        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetInt(GameStates.HIGH_SCORE, _highScore);
            PlayerPrefs.Save();
        }
        
    }

    private void Player_OnCoinCollected(object sender, Player.OnCoinCollectedEventArgs e)
    {
        IncreaseScore(e.coinValue);
        UpdateScoreText();
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetHighScore()
    {
        return _highScore;
    }

    private void IncreaseScore(int value)
    {
        _score += value;
    }

    private void UpdateScoreText()
    {
        scoreText.text = _score.ToString();
    }


    private void OnDestroy()
    {
        if(Player.Instance != null)
        {
            Player.Instance.OnCoinCollected -= Player_OnCoinCollected;
            Player.Instance.OnGameOver -= Player_OnGameOver;
        }
    }
}
