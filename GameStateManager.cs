using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{ 
    
    public static GameStateManager Instance {private set; get;}
    
    public event EventHandler OnGameLostFocus;
    public event EventHandler OnGameRegainedFocus;
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameStates.currentGameState = GameStates.GameState.Paused;
    }

    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnGameOver += Player_OnGameOver;
        }

        if (PausedUI.Instance != null)
        {
            PausedUI.Instance.OnPause += PausedUI_OnPause;
            PausedUI.Instance.OnResume += PausedUI_OnResume;
            PausedUI.Instance.OnTutorialFinished += PausedUI_OnTutorialFinished;
        }
    }

    private void PausedUI_OnTutorialFinished(object sender, EventArgs e)
    {
        GameStates.currentGameState = GameStates.GameState.InGame;
    }

    
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            OnGameLostFocus?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 0f;
        }
        else
        {
            OnGameRegainedFocus?.Invoke(this, EventArgs.Empty);
        }
    }


    private void PausedUI_OnResume(object sender, EventArgs e)
    {
        GameStates.currentGameState = GameStates.GameState.InGame;
    }

    private void PausedUI_OnPause(object sender, EventArgs e)
    {
        GameStates.currentGameState = GameStates.GameState.Paused;
    }

    private void Player_OnGameOver(object sender, EventArgs e)
    {
        GameStates.currentGameState = GameStates.GameState.GameOver;
    }


    private void OnDestroy()
    {
        if(Player.Instance != null)
            Player.Instance.OnGameOver -= Player_OnGameOver;
        if(PausedUI.Instance != null)
        {
            PausedUI.Instance.OnPause -= PausedUI_OnPause;
            PausedUI.Instance.OnResume -= PausedUI_OnResume;
            PausedUI.Instance.OnTutorialFinished -= PausedUI_OnTutorialFinished;
        }
    }
}
