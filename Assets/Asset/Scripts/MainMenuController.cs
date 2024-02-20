/*using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject loadingScreenPanel;
    public Text loadingText;

    // Call this function when the New Game button is clicked
    public void OnNewGameClicked()
    {
        StartCoroutine(LoadNewGame());
    }

    IEnumerator LoadNewGame()
    {
        // Activate the loading screen
        loadingScreenPanel.SetActive(true);
        loadingText.text = "Loading...";

        // Fake a 3 second load time
        yield return new WaitForSeconds(3);

        // Load the Chapter1 scene
        SceneManager.LoadScene("Chapter1");
    }
}
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject loadingScreenPanel;
    public Text loadingText;

    // Call this function when the New Game button is clicked
    public void OnNewGameClicked()
    {
        StartCoroutine(LoadNewGame());
    }

    IEnumerator LoadNewGame()
    {
        // Activate the loading screen
        loadingScreenPanel.SetActive(true);
        loadingText.text = "Loading...";

        // Fake a 3 second load time
        yield return new WaitForSeconds(3);

        // Make sure the game is unpaused when loading a new game
        Time.timeScale = 1f;

        // Reset the pause state before loading the new game scene
        PauseMenuController.ResetPauseState();

        // Load the Chapter1 scene
        SceneManager.LoadScene("Chapter1");

        // Debug the current time scale after loading completes
        Debug.Log("After loading new game, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnNewGameSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnNewGameSceneLoaded;
    }

    private void OnNewGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Chapter1")
        {
            // Find the PlayerController in the new scene
            PlayerController playerController = FindObjectOfType<PlayerController>();

            // Reset player state if the playerController is found
            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }

            // The following line is important to ensure this method doesn't get called again
            // unless we are loading a new game again.
            SceneManager.sceneLoaded -= OnNewGameSceneLoaded;
        }
    }


    // Call this function when the Load Game button is clicked
    public void OnLoadGameClicked()
    {
        // Implement your loading game logic here
    }

    // Call this function when the Exit button is clicked
    public void OnExitClicked()
    {
        // Implement your exit game logic here
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Call this function when you need to return to the main menu from the game
    public void LoadMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}

