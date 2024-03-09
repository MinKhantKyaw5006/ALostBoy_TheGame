using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Ensure this is included for UI elements like Buttons

public class SceneController : MonoBehaviour
{
    // Static singleton property
    public static SceneController Instance { get; private set; }

    void Awake()
    {
        // Check if instance already exists and if it's not this one
        if (Instance != null && Instance != this)
        {
            // Destroy this instance because it's a duplicate
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this and make sure it persists across scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

 

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to the sceneLoaded event to avoid memory leaks
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is the Main Menu
        if (scene.name == "MainMenu")
        {
            SetupMainMenuButtons(); // Setup buttons every time Main Menu is loaded
        }
    }

    /*
    void Update()
    {
        // Check if the Esc key was pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenu();
        }
    }
    */

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadChapter1()
    {
        SceneManager.LoadScene("Chapter1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetupMainMenuButtons()
    {
        // Find the New Game button by tag
        var newGameButtonObject = GameObject.FindWithTag("NewGame"); // Use the tag you assigned
        if (newGameButtonObject != null)
        {
            var buttonComponent = newGameButtonObject.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.RemoveAllListeners(); // Remove existing listeners
                buttonComponent.onClick.AddListener(LoadChapter1); // Add listener to load Chapter 1
            }
        }
        else
        {
            Debug.LogError("NewGameButton with specified tag not found.");
        }
        // Add similar blocks for any other buttons in your Main Menu
    }

 

}
