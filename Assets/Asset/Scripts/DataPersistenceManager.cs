using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    [Header("File Storage config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }


 


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

        //give os persisting data path creation 
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }



    public void NewGame()
    {
        this.gameData = new GameData();

        // Optionally set the starting scene explicitly if not the default
        this.gameData.currentSceneName = "Chapter1"; // Ensure this matches your initial game scene name
        SceneManager.LoadScene(this.gameData.currentSceneName);
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
        //load any save data from  a file using  a data handler
        this.gameData = dataHandler.Load();

        //start a new game if the data is null and we're configured to  initialize data for debugging purposes

        Debug.Log($"Attempting to load game data for scene {SceneManager.GetActiveScene().name}...");

        if (this.gameData == null & initializeDataIfNull)
        {
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

            Debug.Log($"Player position loaded at: {gameData.playerPosition}");
        }
    }
    
    public void SaveGame()
    {

        Debug.Log($"Attempting to save game data for scene {SceneManager.GetActiveScene().name}...");

        //if we don't have any data to save, log a  warning here    
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. a new game needs to be started before  data can be saved");
        }

       


        // Pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log($"Game saved. Player position: {gameData.playerPosition}, Scene: {gameData.currentSceneName}");
        // Here you would add your logic to write the GameData to a file

        //save that data to afile using  the dat handler
        dataHandler.Save(gameData);
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

    public void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        Debug.Log($"Scene {scene.name} loaded with build index {scene.buildIndex}. Loading game data...");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded Called");
        Debug.Log($"Scene {scene.name} unloaded with build index {scene.buildIndex}. Saving game data...");
        SaveGame();
    }

    public bool hasGameData()
    {
        return gameData != null;
    }


   
}

