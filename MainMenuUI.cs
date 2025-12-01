using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;

    private void Awake()
    {
        if(playButton == null) Debug.LogError("Play button not found");
        if(settingsButton == null) Debug.LogError("Settings button not found");
    }
    
    private void Start()
    {
        playButton.onClick.AddListener(PlayPressed);
    }

    private void PlayPressed()
    {
        Loader.LoadScene(Loader.Scene.GameScene);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(PlayPressed);
    }
}
