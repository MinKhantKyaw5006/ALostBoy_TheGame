using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup")]
    [SerializeField] private ConfirmationPopUpMenu confirmationPopUpMenu;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen; // Reference to your loading screen object

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        //initialize saveslots
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    /*
    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        //disable all buttons
        DisableMenuButtons();

        //update the selected profile id to be used for data persistence
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if (!isLoadingGame)
        {
            //create a new game - which will initialize our data to a clean slate
            DataPersistenceManager.instance.NewGame();
        }
       

        //load the scene - which will in turn save the game because of OnSceneUnloaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("Chapter1");
    }
    */

    //new game click error
    /*
    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Disable all buttons
        DisableMenuButtons();

        // Update the selected profile ID for data persistence
        string profileId = saveSlot.GetProfileId();
        DataPersistenceManager.instance.ChangeSelectedProfileId(profileId);

        // Load SaveSlotData for the last played scene
        SaveSlotData saveSlotData = DataPersistenceManager.instance.LoadSaveSlotData(profileId);
        string sceneToLoad = saveSlotData != null && !string.IsNullOrEmpty(saveSlotData.lastPlayedScene) ? saveSlotData.lastPlayedScene : "Chapter1";

        // If not loading a game (i.e., starting new), initialize new game data
        if (!isLoadingGame)
        {
            DataPersistenceManager.instance.NewGame();
        }

        // Load the scene
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
    */


    //original

    //public void OnSaveSlotClicked(SaveSlot saveSlot)
    //{
    //    // Disable all buttons
    //    DisableMenuButtons();

    //    // Update the selected profile ID for data persistence
    //    string profileId = saveSlot.GetProfileId();
    //    DataPersistenceManager.instance.ChangeSelectedProfileId(profileId);

    //    //loading screen
    //    if (loadingScreen != null)
    //    {
    //        loadingScreen.SetActive(true); // Activate the loading screen
    //    }

    //    string sceneToLoad;

    //    // Check whether we are loading a game or starting a new game
    //    if (isLoadingGame)
    //    {

    //        // If loading a game, attempt to load the last played scene
    //        SaveSlotData saveSlotData = DataPersistenceManager.instance.LoadSaveSlotData(profileId);
    //        sceneToLoad = saveSlotData != null && !string.IsNullOrEmpty(saveSlotData.lastPlayedScene)
    //                      ? saveSlotData.lastPlayedScene
    //                      : "Chapter1"; // Fallback to "Chapter1" if no data found
    //    }
    //    else
    //    {
    //        // If starting a new game, ignore last played scene and load "Chapter1"
    //        //DataPersistenceManager.instance.NewGame();
    //        Debug.Log("starting new game called for confirmation");

    //        confirmationPopUpMenu.ActivateMenu(
    //           "Starting a New Game with this slot will override the currently saved data. Are you sure?",
    //           //function to execute if we select 'yes'
    //           () =>
    //           {
    //               DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
    //               DataPersistenceManager.instance.NewGame();
    //           },
    //           //function to execute if we select 'no'
    //           () =>
    //           {
    //               this.ActivateMenu(isLoadingGame);
    //           }
    //       );
    //        sceneToLoad = "Chapter1";
    //    }

    //    ////save game data
    //    ////DataPersistenceManager.instance.SaveGame();

    //    //// Load the determined scene
    //    DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
    //    DataPersistenceManager.instance.NewGame();


    //    SceneManager.LoadSceneAsync(sceneToLoad);
    //    //SaveGameAndLoadGame();
    //}

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Disable all buttons
        DisableMenuButtons();

        // Get the current date and time
        System.DateTime currentDateTime = System.DateTime.Now;

        // Format the date and time string
        string formattedDateTime = currentDateTime.ToString("dd/MM/yyyy HH:mm:ss");

        // Log to the debug console
        Debug.Log("Save slot clicked at: " + formattedDateTime);

        // Update the selected profile ID for data persistence
        string profileId = saveSlot.GetProfileId();
        DataPersistenceManager.instance.ChangeSelectedProfileId(profileId);

        // Load the save slot data to check if it contains saved game data
        SaveSlotData saveSlotData = DataPersistenceManager.instance.LoadSaveSlotData(profileId);

        // Check if we're starting a new game or loading a saved game
        if (!isLoadingGame)
        {
            // If there is existing data in the save slot, show the confirmation popup
            if (saveSlotData != null)
            {
                confirmationPopUpMenu.ActivateMenu(
                    "Starting a New Game with this slot will override the currently saved data. Are you sure?",
                    // Function to execute if we select 'yes'
                    () =>
                    {
                        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                        DataPersistenceManager.instance.NewGame();

                        // Update the last updated timestamp
                        GameData gameData = new GameData(); // Assuming you instantiate GameData here
                        gameData.lastUpdated = ((DateTimeOffset)currentDateTime).ToUnixTimeMilliseconds();

                        // After confirming, load the new game scene
                        LoadGame("Chapter1");
                    },
                    // Function to execute if we select 'no'
                    () =>
                    {
                        this.ActivateMenu(isLoadingGame);
                    }
                );
            }
            else
            {
                // No saved data, so just start a new game
                DataPersistenceManager.instance.NewGame();
                LoadGame("Chapter1");
            }
        }
        else
        {
            // Load the saved game if the mode is loading
            string sceneToLoad = saveSlotData != null && !string.IsNullOrEmpty(saveSlotData.lastPlayedScene)
                ? saveSlotData.lastPlayedScene
                : "Chapter1"; // Fallback to "Chapter1" if no data found

            LoadGame(sceneToLoad);
        }
    }

    // Utility method to load the game and show the loading screen
    private void LoadGame(string sceneToLoad)
    {
        // Show the loading screen if it exists
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Load the scene asynchronously
        SceneManager.LoadSceneAsync(sceneToLoad);
    }


    private void SaveGameAndLoadGame()
    {
        //save game data
        //DataPersistenceManager.instance.SaveGame();

        // Load the determined scene
        //SceneManager.LoadSceneAsync(sceneToLoad);
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        confirmationPopUpMenu.ActivateMenu(
            "Are you sure you want to delete this saved data?",
            //function to execute if we select 'yes'
            () =>
            {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu(isLoadingGame);
            },
            //function to execute if we select 'no'
            () =>
            {
                ActivateMenu(isLoadingGame);
            }
            );


       
    }





    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    //original
    /*
    public void ActivateMenu(bool isLoadingGame)
    {
        //Set this menu to be active
        this.gameObject.SetActive(true);

        //set mode
        this.isLoadingGame = isLoadingGame;

        //load all of  the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        //loop through each save slot in the UI and set the content appropriately
        GameObject firstSelected = backButton.gameObject;

        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            //disable interaction if not data in saveslot
            //enable if have data
            if(profileData == null && isLoadingGame)
            {
                saveSlot.Setinteractable(false);
            }
            else
            {
                saveSlot.Setinteractable(true);
                //defaulting saveslot position
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }
        //set the first selected button
        StartCoroutine(this.SetFirstSelected(firstSelected));
    }
    */

    public void ActivateMenu(bool isLoadingGame)
    {
        //Set this menu to be active
        this.gameObject.SetActive(true);

        //set mode
        this.isLoadingGame = isLoadingGame;

        //load all of  the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        //ensure the back button is enabled when we activate the menu
        backButton.interactable = true;

        //loop through each save slot in the UI and set the content appropriately
        GameObject firstSelected = backButton.gameObject;

        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            string profileId = saveSlot.GetProfileId(); // Get the profile ID for each save slot
            profilesGameData.TryGetValue(profileId, out profileData);

            // Pass both profileData and profileId to the SetData method
            saveSlot.SetData(profileData, profileId);

            //disable interaction if not data in saveslot
            //enable if have data
            if (profileData == null && isLoadingGame)
            {
                saveSlot.Setinteractable(false);
            }
            else
            {
                saveSlot.Setinteractable(true);
                //defaulting saveslot position
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }
        //set the first selected button
        StartCoroutine(this.SetFirstSelected(firstSelected));
    }


    public void DeactivateMenu() 
    {
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach(SaveSlot saveSlot in saveSlots)
        {
            saveSlot.Setinteractable(false);
        }
        backButton.interactable = false;
    }
}
