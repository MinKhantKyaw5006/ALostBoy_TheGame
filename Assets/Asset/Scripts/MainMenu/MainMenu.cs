using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button quitGameButton; // Add this line

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen; // Add this line


    ///original

    public void Start()
    {
        

        //check if game has data, if no data, initialize new game
        if (!DataPersistenceManager.instance.HasGameData())
        {
            //disable continue button   
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;

        }
       
    }
    
    

    

    

    /*
    public void OnNewGameClicked()
    {
        DisableMenuButton();

        Debug.Log("New Game Clicked");
        //create a new game - which will initialize game data
        DataPersistenceManager.instance.NewGame();
        //load the gameplay scene - which will in turn save the game because of
        //onsceneunloaded() in the datapersistencemanager
        SceneManager.LoadSceneAsync("Chapter1");
    }
    */

    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();

  
    }

    public void OnContinueGameClicked()
    {
        DisableMenuButton();

        Debug.Log("Continue Game Clicked");
        //load the next scene - which will  in turn load the game  because of
        // Explicitly load game data before loading the gameplay scene
        DataPersistenceManager.instance.LoadGame();

        //save game data
        DataPersistenceManager.instance.SaveGame();


        // Determine which scene to load based on saved scene data
        string sceneToLoad = DataPersistenceManager.instance.DetermineSceneToLoad();
        Debug.Log($"Loading scene: {sceneToLoad}");

        // Activate the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        SceneManager.LoadSceneAsync(sceneToLoad);

        // Assuming LoadGame sets up necessary game state, then load the gameplay scene
        //SceneManager.LoadSceneAsync("Chapter1");
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }


    // New method for handling quit game button click
    public void OnQuitGameClicked()
    {
        Debug.Log("Quit Game Clicked");

        // Quit the application
        Application.Quit();

        // If running in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    //disabling function
    private void DisableMenuButton()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
        
    }

    public void ActivateMenu()
    {
        //enable savemenu when activate
        this.gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        //disable savemenu when activate
        this.gameObject.SetActive(false);
    }
}
