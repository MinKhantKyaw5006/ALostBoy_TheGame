using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;

    private void Start()
    {
        if (!DataPersistenceManager.instance.hasGameData())
        {
            continueGameButton.interactable = false;
        }
    }
    public void OnNewGameClicked()
    {
        DisableMenuButtons();
        Debug.Log("New Game Clicked");
        //create new game - which will initialize our game data
        DataPersistenceManager.instance.NewGame();
        //load the gameplay scene - which will turn on save the game because of
        //onsceneunloaded method in the datapersistence manager
        SceneManager.LoadSceneAsync("Chapter1");
    }

    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        Debug.Log("Continue Game Clicked");
        // Load the next scene - which will load the game because of
        //On SceneLoaded() in the Datapersistence Manager
        SceneManager.LoadSceneAsync("Chapter1");
    }

    private void DisableMenuButtons() 
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
