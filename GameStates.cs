
public static class GameStates
{
    public enum GameState
    {
        MainMenu,
        InGame,
        GameOver,
        Paused
    }
    
    public static GameState currentGameState = GameState.MainMenu;
    
}
