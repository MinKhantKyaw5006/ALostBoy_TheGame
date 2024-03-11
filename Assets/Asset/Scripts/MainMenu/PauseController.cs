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
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    public void RestartChapter()
    {
        // Ensure DataPersistenceManager instance is available
        if (DataPersistenceManager.instance != null)
        {
            // Indicate that a reset is requested
            DataPersistenceManager.instance.shouldReset = true;

            // Save the game to apply the reset immediately
            DataPersistenceManager.instance.SaveGame();

            // Ensure game time is resumed before reloading
            Time.timeScale = 1f;

            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogError("DataPersistenceManager instance not found. Cannot restart chapter.");
        }
    }


    public void RestartFromLastCheckpoint()
    {
        // Check if DataPersistenceManager instance is available
        if (DataPersistenceManager.instance != null)
        {
            // Load the most recent game state, which should be the last checkpoint
            DataPersistenceManager.instance.LoadGame();

            Debug.Log("Restarted from last checkpoint");
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;


        }
        else
        {
            Debug.LogError("DataPersistenceManager instance not found. Cannot restart from last checkpoint.");
        }
    }





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
