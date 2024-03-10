using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.IO;



/*
public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName = "MyGame.json";
    
    [SerializeField] private string sceneDataFileName = "sceneData.json"; // Different file for scene data

    private SceneData sceneData;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public SceneDataHandler sceneDataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than  one Data Peristence Manager in the scene.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.gameData = new GameData(); // Make sure this line is executed

        sceneDataHandler = new SceneDataHandler(Application.persistentDataPath, sceneDataFileName);
        sceneData = sceneDataHandler.LoadSceneData(); // Load scene data at start
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }



    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        SceneData existingSceneData = sceneDataHandler.LoadSceneData(); // Always load the existing scene data first

        // Only update scene data under specific conditions to avoid overwriting meaningful gameplay scenes with "GameMainMenu"
        if (existingSceneData != null)
        {
            // Case: First load of "GameMainMenu" in a new session should not overwrite an existing gameplay scene
            if (sceneName == "GameMainMenu" && !string.IsNullOrEmpty(existingSceneData.previousScene) && existingSceneData.previousScene != "GameMainMenu")
            {
                // Do not update scene data if we're just seeing "GameMainMenu" upon starting the game
                // but still load existing scene data to preserve the transition from a gameplay scene to "GameMainMenu"
            }
            else
            {
                // Normal case: Update scene data for gameplay transitions or if going from "GameMainMenu" to a gameplay scene
                sceneData.previousScene = existingSceneData.currentScene;
                sceneData.currentScene = sceneName;
                Debug.Log($"Scene transition: Previous - {sceneData.previousScene}, Current - {sceneData.currentScene}");
                sceneDataHandler.SaveSceneData(sceneData); // Save updated scene data
            }
        }
        else
        {
            // This block executes if no existing scene data was found (e.g., first game launch ever)
            sceneData.previousScene = ""; // Or some default initial value
            sceneData.currentScene = sceneName;
            Debug.Log($"First scene load, setting scene data: Current - {sceneData.currentScene}");
            sceneDataHandler.SaveSceneData(sceneData); // Save initial scene data
        }

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame(); // Continue to load game data
    }





    public void OnSceneUnloaded(Scene scene)
    {
        // Optional: Handle any additional logic needed when a scene unloads
        Debug.Log("OnSceneUnloaded called");

        SaveGame();
    }




    public void NewGame()
    {
        this.gameData = new GameData();
    }


    
    public void LoadGame()
    {
        //load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

        
        Debug.Log("loading game");

        //start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        //if no data can be loaded, initialize to new game
        if(this.gameData == null)
        {
            Debug.Log("No data was found. A new game needs to be started before data can be loaded.");
            return;
        }
        //push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }




    public void SaveGame()
    {
        //if we dont have any data to save, log a warning here
        if(this.gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return;
        }
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // Log the player's position at the time of saving (assuming player's data is part of gameData).
        //Debug.Log($"Player position saved: {gameData.playerPosition}");

        // save that data to a file using file data handler
        dataHandler.Save(gameData);

        

        // Known issue: MissingReferenceException occurs when transitioning back to the main menu because
        // objects implementing IDataPersistence (e.g., PlayerController) are destroyed before the DataPersistenceManager
        // completes the save operation. Currently, this does not affect game functionality and is considered a low-priority issue.

    }
    

  


    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Quit Saved");
    }

    public bool HasGameData()
    {
        return gameData != null;
    }



    public string DetermineSceneToLoad()
    {
        Debug.Log($"[DetermineSceneToLoad] Current Scene: {sceneData.currentScene}, Previous Scene: {sceneData.previousScene}");

        // If the current scene is "MainMenu" and there's a valid previous scene that is not "MainMenu", prefer the previous scene.
        if (sceneData.currentScene == "GameMainMenu" && !string.IsNullOrEmpty(sceneData.previousScene) && sceneData.previousScene != "GameMainMenu")
        {
            Debug.Log($"[DetermineSceneToLoad] Decided to load (Previous Scene): {sceneData.previousScene}");
            return sceneData.previousScene;
        }
        // If neither the current scene nor the previous scene is "MainMenu", prefer the current scene.
        else if (!string.IsNullOrEmpty(sceneData.currentScene) && sceneData.currentScene != "GameMainMenu" && !string.IsNullOrEmpty(sceneData.previousScene) && sceneData.previousScene != "GameMainMenu")
        {
            Debug.Log($"[DetermineSceneToLoad] Decided to load (Current Scene): {sceneData.currentScene}");
            return sceneData.currentScene;
        }
        // Otherwise, if the current scene is not "MainMenu" and there's no valid previous scene, load the current scene.
        else if (!string.IsNullOrEmpty(sceneData.currentScene) && sceneData.currentScene != "GameMainMenu")
        {
            Debug.Log($"[DetermineSceneToLoad] Decided to load (Current Scene): {sceneData.currentScene}");
            return sceneData.currentScene;
        }
        // Default to a specific scene if none match your criteria or both are "MainMenu"
        Debug.Log("[DetermineSceneToLoad] Defaulting to 'Chapter1'");
        return "Chapter1"; // Adjust this to your game's default or initial scene
    }




}
*/

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";

   

    [Header("File Storage Config")]
    [SerializeField] private string fileName = "MyGame.json";

    [SerializeField] private string sceneDataFileName = "sceneData.json"; // Different file for scene data
    public bool shouldReset = false; // Add this field to DataPersistenceManager


    private SceneData sceneData;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public SceneDataHandler sceneDataHandler;
    private string selectedProfileId = "";
    // New field to indicate a reset is requested
    public bool resetGameOnLoad = false;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than  one Data Peristence Manager in the scene.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfile();
        this.gameData = new GameData(); // Make sure this line is executed

        sceneDataHandler = new SceneDataHandler(Application.persistentDataPath, sceneDataFileName);
        sceneData = sceneDataHandler.LoadSceneData(); // Load scene data at start

        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id" + testSelectedProfileId);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    /*
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        SceneData existingSceneData = sceneDataHandler.LoadSceneData(); // Always load the existing scene data first

        // Only update scene data under specific conditions to avoid overwriting meaningful gameplay scenes with "GameMainMenu"
        if (existingSceneData != null)
        {
            // Case: First load of "GameMainMenu" in a new session should not overwrite an existing gameplay scene
            if (sceneName == "GameMainMenu" && !string.IsNullOrEmpty(existingSceneData.previousScene) && existingSceneData.previousScene != "GameMainMenu")
            {
                // Do not update scene data if we're just seeing "GameMainMenu" upon starting the game
                // but still load existing scene data to preserve the transition from a gameplay scene to "GameMainMenu"
            }
            else
            {
                // Normal case: Update scene data for gameplay transitions or if going from "GameMainMenu" to a gameplay scene
                sceneData.previousScene = existingSceneData.currentScene;
                sceneData.currentScene = sceneName;
                Debug.Log($"Scene transition: Previous - {sceneData.previousScene}, Current - {sceneData.currentScene}");
                sceneDataHandler.SaveSceneData(sceneData); // Save updated scene data
            }
        }
        else
        {
            // This block executes if no existing scene data was found (e.g., first game launch ever)
            sceneData.previousScene = ""; // Or some default initial value
            sceneData.currentScene = sceneName;
            Debug.Log($"First scene load, setting scene data: Current - {sceneData.currentScene}");
            sceneDataHandler.SaveSceneData(sceneData); // Save initial scene data
        }

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame(); // Continue to load game data
    }
    */

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene {scene.name} loaded, checking if game data reset is requested");
        


        string sceneName = scene.name;
        SceneData existingSceneData = sceneDataHandler.LoadSceneData();

        if (existingSceneData != null)
        {
            if (sceneName == "GameMainMenu" && !string.IsNullOrEmpty(existingSceneData.previousScene) && existingSceneData.previousScene != "GameMainMenu")
            {
                // Skip updating scene data for "GameMainMenu" if coming from another scene
            }
            else
            {
                sceneData.previousScene = existingSceneData.currentScene;
                sceneData.currentScene = sceneName;
                Debug.Log($"Scene transition: Previous - {sceneData.previousScene}, Current - {sceneData.currentScene}");
                sceneDataHandler.SaveSceneData(sceneData);
            }
        }
        else
        {
            sceneData.previousScene = "";
            sceneData.currentScene = sceneName;
            Debug.Log($"First scene load, setting scene data: Current - {sceneData.currentScene}");
            sceneDataHandler.SaveSceneData(sceneData);
        }

        // New logic to handle SaveSlot.json
        // Check if the scene is not "GameMainMenu" before saving the scene name to SaveSlot.json
        if (sceneName != "GameMainMenu" && !string.IsNullOrEmpty(selectedProfileId))
        {
            SaveSceneNameToSaveSlotJson(selectedProfileId, sceneName);
        }

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame(); // Continue to load game data
        
    }

    private void SaveSceneNameToSaveSlotJson(string profileId, string sceneName)
    {
        // Assuming fileDataHandler is an instance of FileDataHandler already defined in your class.
        SaveSlotData saveSlotData = new SaveSlotData { lastPlayedScene = sceneName };
        dataHandler.SaveSaveSlotData(profileId, saveSlotData); // Use the instance to call the method
        Debug.Log($"Saved '{sceneName}' to SaveSlot.json for profile {profileId}");
    }





    /*
    public void OnSceneUnloaded(Scene scene)
    {
        // Optional: Handle any additional logic needed when a scene unloads
        Debug.Log("OnSceneUnloaded called");

        SaveGame();
    }
    */


    //original
    /*
    public void NewGame()
    {
        Debug.Log("New game started, resetting game data including player position");
        this.gameData = new GameData();
  
    }
    */

    public void NewGame()
    {
        Debug.Log("New game started, resetting game data including player position.");
        gameData = new GameData();
        shouldReset = true; // Indicate that we've started a new game
    }




    /*
    public void LoadGame()
    {
        //return right away if data  persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        //load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileId);


        Debug.Log("loading game");

        //start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();

        }

        //if no data can be loaded, initialize to new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A new game needs to be started before data can be loaded.");
            return;
        }
        //push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    */

    public void LoadGame()
    {
        if (disableDataPersistence) return;

        // Check if we should reset the game data based on the flag
        if (shouldReset)
        {
            Debug.Log("Resetting game data as requested.");
            NewGame();
            shouldReset = false; // Reset the flag after handling
            SaveGame(); // Optionally save immediately to persist the reset state
        }
        else
        {
            // Proceed with loading saved data
            gameData = dataHandler.Load(selectedProfileId);

            if (gameData == null)
            {
                Debug.Log("No saved data was found. Starting a new game.");
                if (initializeDataIfNull)
                {
                    NewGame(); // Optionally start a new game if configured to do so
                }
                else
                {
                    // Optionally, handle disabling continue/load game in main menu here
                    // This might involve setting a flag or calling a method on your menu controller
                }
            }
            else
            {
                Debug.Log("Loading game data.");
                foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
                {
                    dataPersistenceObj.LoadData(gameData);
                }
            }
        }
    }








    public void SaveGame()
    {
        //return right away if data  persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        //if we dont have any data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return;
        }
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // Log the player's position at the time of saving (assuming player's data is part of gameData).
        //Debug.Log($"Player position saved: {gameData.playerPosition}");

        // save that data to a file using file data handler
        dataHandler.Save(gameData, selectedProfileId);

        //timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        //IMPORTANT TO NOTE!!!!!!
        // Known issue: MissingReferenceException occurs when transitioning back to the main menu because
        // objects implementing IDataPersistence (e.g., PlayerController) are destroyed before the DataPersistenceManager
        // completes the save operation. Currently, this does not affect game functionality and is considered a low-priority issue.

    }





    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Quit Saved");
    }

    public bool HasGameData()
    {
        return gameData != null;
    }



    public string DetermineSceneToLoad()
    {
        Debug.Log($"[DetermineSceneToLoad] Current Scene: {sceneData.currentScene}, Previous Scene: {sceneData.previousScene}");

        // If the current scene is "MainMenu" and there's a valid previous scene that is not "MainMenu", prefer the previous scene.
        if (sceneData.currentScene == "GameMainMenu" && !string.IsNullOrEmpty(sceneData.previousScene) && sceneData.previousScene != "GameMainMenu")
        {
            Debug.Log($"[DetermineSceneToLoad] Decided to load (Previous Scene): {sceneData.previousScene}");
            return sceneData.previousScene;
        }
        // If neither the current scene nor the previous scene is "MainMenu", prefer the current scene.
        else if (!string.IsNullOrEmpty(sceneData.currentScene) && sceneData.currentScene != "GameMainMenu" && !string.IsNullOrEmpty(sceneData.previousScene) && sceneData.previousScene != "GameMainMenu")
        {
            Debug.Log($"[DetermineSceneToLoad] Decided to load (Current Scene): {sceneData.currentScene}");
            return sceneData.currentScene;
        }
        // Otherwise, if the current scene is not "MainMenu" and there's no valid previous scene, load the current scene.
        else if (!string.IsNullOrEmpty(sceneData.currentScene) && sceneData.currentScene != "GameMainMenu")
        {
            Debug.Log($"[DetermineSceneToLoad] Decided to load (Current Scene): {sceneData.currentScene}");
            return sceneData.currentScene;
        }
        // Default to a specific scene if none match your criteria or both are "MainMenu"
        Debug.Log("[DetermineSceneToLoad] Defaulting to 'Chapter1'");
        return "Chapter1"; // Adjust this to your game's default or initial scene
    }

    //get data for profileid all at once from other scripts method to get all profile game data

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        //update the profile to  use for saving and loading
        this.selectedProfileId = newProfileId;
        //load the game which will use that profile, updating our game data accordingly
        LoadGame();
    }


    // Method to load SaveSlotData
    public SaveSlotData LoadSaveSlotData(string profileId)
    {
        return dataHandler.LoadSaveSlotData(profileId); // Utilize existing dataHandler
    }

    public void DeleteGameDataFile()
    {
        string profilePath = Path.Combine(dataHandler.DataDirPath, selectedProfileId);
        string filePath = Path.Combine(profilePath, fileName);

        if (File.Exists(filePath))
        {
            Debug.Log($"Deleting game data file: {filePath}");
            File.Delete(filePath);
        }
    }




}
