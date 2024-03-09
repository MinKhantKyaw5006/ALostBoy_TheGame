using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        //initialize saveslots
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

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

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }



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
