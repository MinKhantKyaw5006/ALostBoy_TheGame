using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotMenu saveSlotsMenu;


    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button chapter2button;
    [SerializeField] private Button loadGameButton;

    private void Start()
    {
        DisablebuttonsDependingOnData();
    }
    public void OnNewGameClicked()
    {
        /* original straightforward code
        DisableMenuButtons();
        Debug.Log("New Game Clicked");
        //create new game - which will initialize our game data
        DataPersistenceManager.instance.NewGame();
        //load the gameplay scene - which will turn on save the game because of
        //onsceneunloaded method in the datapersistence manager
        SceneManager.LoadSceneAsync("Chapter1");
        */

        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    /*
    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        Debug.Log("Continue Game Clicked");
        //save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();
        // Load the next scene - which will load the game because of
        //On SceneLoaded() in the Datapersistence Manager
        SceneManager.LoadSceneAsync("Chapter2");
    }
    */
    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        Debug.Log("Continue Game Clicked");

        // Assuming DataPersistenceManager.instance.LoadGame() has been called earlier
        // and it sets the current scene name in the game data correctly
        string lastScene = DataPersistenceManager.instance.GetLastSceneName();

        // Check if we have a valid scene name to load
        if (!string.IsNullOrEmpty(lastScene))
        {
            SceneManager.LoadSceneAsync(lastScene);
        }
        else
        {
            Debug.LogError("Failed to load the last scene. Scene name is invalid.");
        }
    }




    public void Chapter2Scene()
    {
        DisableMenuButtons();
        Debug.Log("Chapter2 Clicked");
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("Chapter2");
    }

    private void DisableMenuButtons() 
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
        DisablebuttonsDependingOnData();
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    private void DisablebuttonsDependingOnData()
    {
        if (!DataPersistenceManager.instance.hasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }
}
