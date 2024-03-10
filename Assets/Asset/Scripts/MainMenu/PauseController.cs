using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign this in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeInHierarchy)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    /*
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure game time is resumed
        SceneManager.LoadScene("GameMainMenu");
        
    }
    */

    /*
    public void LoadMainMenu()
    {
        // Save the game before going back to the main menu
        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
        }
        else
        {
            Debug.LogError("DataPersistenceManager instance not found. Game not saved.");
        }

        Time.timeScale = 1f; // Ensure game time is resumed
        SceneManager.LoadScene("GameMainMenu");


    }
    */


    //original
    
    public void LoadMainMenu()
    {
        // Check if DataPersistenceManager and SceneDataHandler instances are available
        if (DataPersistenceManager.instance != null && DataPersistenceManager.instance.sceneDataHandler != null)
        {
            // Load current scene data to update it for transitioning back to the main menu
            SceneData currentSceneData = DataPersistenceManager.instance.sceneDataHandler.LoadSceneData();

            // Set the previous scene to the current game scene before transitioning to the main menu
            if (currentSceneData != null)
            {
                currentSceneData.previousScene = currentSceneData.currentScene;
                currentSceneData.currentScene = "GameMainMenu";
                DataPersistenceManager.instance.sceneDataHandler.SaveSceneData(currentSceneData);
                Debug.Log($"Updated scene data for transition to main menu: Previous - {currentSceneData.previousScene}, Current - {currentSceneData.currentScene}");
            }

            // Save the game state before transitioning to the main menu
            //DataPersistenceManager.instance.SaveGame();

            // Ensure game time is resumed
            Time.timeScale = 1f;

            // Load the main menu scene
            SceneManager.LoadScene("GameMainMenu");
        }
        else
        {
            Debug.LogError("DataPersistenceManager or SceneDataHandler instance not found. Game not saved.");
        }
    }



}
