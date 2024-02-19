/*using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI; // The pause menu panel
    private static bool gameIsPaused = false; // Static variable to track the pause state

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        gameIsPaused = !gameIsPaused;
        pauseMenuUI.SetActive(gameIsPaused);
        Time.timeScale = gameIsPaused ? 0f : 1f;

        // Add a Debug.Log statement to print the time scale
        Debug.Log("Time.timeScale is now: " + Time.timeScale);
    }

    public void Resume()
    {
        TogglePauseMenu();
        // It's also good to check after calling Resume
        Debug.Log("Resuming game, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }

    public void RestartChapter()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;

        Debug.Log("Restarting chapter, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f; // Make sure to reset the time scale when loading the main menu
        Debug.Log("Loading main menu, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }

    // Public method or property to access the game pause state from other scripts
    public static bool IsGamePaused()
    {
        return gameIsPaused;
    }



}
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController instance; // Singleton instance
    public GameObject pauseMenuUI; // The pause menu panel
    private static bool gameIsPaused = false; // Static variable to track the pause state

    private void Awake()
    {
        // Check if instance already exists and set it if it doesn't
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // Ensure that there's only one instance of this object in the game
            Destroy(gameObject);
        }

        // Optionally, make this object persistent between scene loads
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        gameIsPaused = !gameIsPaused;
        pauseMenuUI.SetActive(gameIsPaused);
        Time.timeScale = gameIsPaused ? 0f : 1f;
        Debug.Log("Time.timeScale is now: " + Time.timeScale);
    }

    public void Resume()
    {
        TogglePauseMenu();
        Debug.Log("Resuming game, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }

    public void RestartChapter()
    {
        // Reset the pause state before restarting the chapter
        ResetPauseState();

        // Reload the current scene to restart the chapter
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;

        // Debug to confirm the time scale is reset
        Debug.Log("Restarting chapter, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }


    public void LoadMainMenu()
    {
        // Reset the pause state before loading the main menu
        ResetPauseState();

        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f; // Make sure to reset the time scale when loading the main menu
        Debug.Log("Loading main menu, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }


    public static bool IsGamePaused()
    {
        return gameIsPaused;
    }

    public static void ResetPauseState()
    {
        gameIsPaused = false;
        if (instance != null)
        {
            instance.pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        Debug.Log("Pause state reset, Time.timeScale should be 1. Current value: " + Time.timeScale);
    }
}
