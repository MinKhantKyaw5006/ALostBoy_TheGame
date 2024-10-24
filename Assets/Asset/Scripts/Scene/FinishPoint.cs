//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class FinishPoint : MonoBehaviour

//{

//    [SerializeField] private GameObject loadingScreen; // Reference to your loading screen

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        // Activate the loading screen
//        if (loadingScreen != null)
//        {
//            loadingScreen.SetActive(true);
//        }
//        if (collision.CompareTag("Player"))
//        {
//            // Check if the DataPersistenceManager instance is available
//            if (DataPersistenceManager.instance != null)
//            {
//                // Set the flag to reset game data for the next playthrough
//                DataPersistenceManager.instance.shouldReset = true;

//                // Optionally, reset data immediately if needed
//                // DataPersistenceManager.instance.NewGame();
//            }
//            else
//            {
//                Debug.LogError("DataPersistenceManager instance not found.");
//            }

//            // Determine if the current level is the last level
//            // Assuming the last level has the highest build index
//            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
//            {
//                // If this is the last level, load the main menu (index 0)
//                SceneManager.LoadScene(0);
//            }
//            else
//            {
//                // If not the last level, proceed to the next level
//                SceneController.instance.NextLevel();
//            }
//        }
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Make sure to include the TextMeshPro namespace if using TextMeshPro

public class FinishPoint : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen1; // Reference to your loading screen for levels
    [SerializeField] private GameObject loadingScreen2; // Reference to your loading screen for the end
    [SerializeField] private TextMeshProUGUI loadingText; // Reference to the TextMeshPro text for loading screen

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

            // Activate the appropriate loading screen based on the current scene
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                // Activate loading screen 2 for the last scene
                loadingScreen2.SetActive(true);
                StartCoroutine(ShowEndMessages());
            }
            else
            {
                // Activate loading screen 1 for other levels
                loadingScreen1.SetActive(true);
                StartCoroutine(LoadNextLevel());
            }
        }
    }

    private IEnumerator LoadNextLevel()
    {
        // Optionally, you can add loading text for the first loading screen
        loadingText.text = "Loading Next Level...";
        loadingText.alpha = 1; // Ensure the text is fully visible

        // Wait for a few seconds or implement a loading process here
        yield return new WaitForSeconds(2f);

        // Determine if not the last level, proceed to the next level
        SceneController.instance.NextLevel();
    }

    private IEnumerator ShowEndMessages()
    {
        // Show the first message
        loadingText.text = "The lost boy is finally free from the forest";
        loadingText.alpha = 1; // Ensure the text is fully visible

        // Wait for a few seconds before fading
        yield return new WaitForSeconds(2f);

        // Fade out the text
        for (float alpha = 1; alpha >= 0; alpha -= Time.deltaTime)
        {
            loadingText.alpha = alpha;
            yield return null; // Wait for the next frame
        }

        // Show the second message
        loadingText.text = "Thank you for playing our game!";
        loadingText.alpha = 0; // Start invisible

        // Fade in the text
        for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            loadingText.alpha = alpha;
            yield return null; // Wait for the next frame
        }

        // Wait for a few seconds before loading the main menu
        yield return new WaitForSeconds(2f);

        // Fade out the second message
        for (float alpha = 1; alpha >= 0; alpha -= Time.deltaTime)
        {
            loadingText.alpha = alpha;
            yield return null; // Wait for the next frame
        }

        // Load the main menu (index 0)
        SceneManager.LoadScene(0);
    }
}
