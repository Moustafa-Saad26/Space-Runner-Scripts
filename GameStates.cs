
public static class GameStates
{
    public enum GameState
    {
        InGame,
        Paused,
        GameOver
    }
    
    public static GameState currentGameState;
    
    public const string SFX_VOLUME = "SFX_VOLUME";
    public const string MUSIC_VOLUME = "MUSIC_VOLUME";
    
}
