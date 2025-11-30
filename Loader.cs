using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    
    public enum Scene
    {
        LoadingScene,
        MainMenuScene,
        GameScene
    }

    private static Scene targetScene;
    
    public static void LoadScene(Scene scene)
    {
        targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}