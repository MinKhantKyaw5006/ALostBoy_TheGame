using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour

{

    [SerializeField] private GameObject loadingScreen; // Reference to your loading screen

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Activate the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        if (collision.CompareTag("Player"))
        {
            // Check if the DataPersistenceManager instance is available
            if (DataPersistenceManager.instance != null)
            {
                // Set the flag to reset game data for the next playthrough
                DataPersistenceManager.instance.shouldReset = true;

                // Optionally, reset data immediately if needed
                // DataPersistenceManager.instance.NewGame();
            }
            else
            {
                Debug.LogError("DataPersistenceManager instance not found.");
            }

            // Determine if the current level is the last level
            // Assuming the last level has the highest build index
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                // If this is the last level, load the main menu (index 0)
                SceneManager.LoadScene(0);
            }
            else
            {
                // If not the last level, proceed to the next level
                SceneController.instance.NextLevel();
            }
        }
    }
}
