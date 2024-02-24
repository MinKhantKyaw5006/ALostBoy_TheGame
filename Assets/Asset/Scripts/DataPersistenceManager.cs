using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";
    [Header("File Storage config")]
    [SerializeField] private string fileName;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;
 


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.Destorying the newest one ");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently diabled!");
        }

        //give os persisting data path creation 
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        Debug.Log($"GameData found: {hasGameData()}");

        InitializeSelectedProfileId();
    }



    public void NewGame()
    {
        this.gameData = new GameData();

        // Optionally set the starting scene explicitly if not the default
        //this.gameData.currentSceneName = "Chapter1"; // Ensure this matches your initial game scene name
        //SceneManager.LoadScene(this.gameData.currentSceneName);
    }



    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    
    public void LoadGame()
    {
        //return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        //load any save data from  a file using  a data handler
        this.gameData = dataHandler.Load(selectedProfileId);

        //start a new game if the data is null and we're configured to  initialize data for debugging purposes

        Debug.Log($"Attempting to load game data for scene {SceneManager.GetActiveScene().name}...");

        if (this.gameData == null & initializeDataIfNull)
        {
            Debug.Log("No data found, new to start from new game");
            NewGame();
        }



        // If no data can be loaded, dont continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A new game need to be started before data can be loaded");
            return;
        }
        else
        {
            // Push the loaded data to all other scripts that need it
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }

           
        }
    }
    
    public void SaveGame()
    {
        //return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        Debug.Log($"Attempting to save game data for scene {SceneManager.GetActiveScene().name}...");

        //if we don't have any data to save, log a  warning here    
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. a new game needs to be started before  data can be saved");
        }

       


        // Pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData( gameData);
        }

        //Debug.Log($"Game saved. Player position: {gameData.playerPosition}, Scene: {gameData.currentSceneName}");
        // Here you would add your logic to write the GameData to a file

        //timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        //save that data to afile using  the dat handler
        dataHandler.Save(gameData, selectedProfileId);
    }
    

    


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        Debug.Log($"Scene {scene.name} loaded with build index {scene.buildIndex}. Loading game data...");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        //start up the auto  saving coroutine
        if(autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }



    public bool hasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
    
    public void ChangeSelectedProfileId(string newProfileId)
    {
        //update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        //load the game, which will use that  profile, updating our game, data accordingly
        LoadGame();

    }

    public void DeleteProfileData(string profileId)
    {
        //delete the data for this profile id
        dataHandler.Delete(profileId);
        //initialize the selected profile id
        InitializeSelectedProfileId();
        //reload the game so that  our data matches the newly selected profile
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Override selected profile id with  test id: " + testSelectedProfileId);
        }
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            Debug.Log("Auto Saved Game");
        }
    }

    public string GetLastSceneName()
    {
        if (gameData != null && !string.IsNullOrEmpty(gameData.currentSceneName))
        {
            return gameData.currentSceneName;
        }
        else
        {
            return null; // Or return a default scene name if you prefer
        }
    }
}

