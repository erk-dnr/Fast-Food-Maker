using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] Button playMultiplayerButton;
    [SerializeField] Button playSingleplayerButton;
    [SerializeField] Button quitButton;

    void Awake()
    {
        playMultiplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        
        playSingleplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        // reset time scale after pausing the game and going back to MainMenu
        Time.timeScale = 1f;
    }
}
