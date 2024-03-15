/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Add this line for scene management

public class HeartController : MonoBehaviour
{
    public RectTransform heartFill; // Assign this in the inspector
    public float maxWidth; // Maximum width of the heartFill when at full health
    //public RectTransform heartFillRectTransform; // Assign in Inspector


    private PlayerController player;

    void Start()
    {
        player = PlayerController.Instance;
        PlayerController.Instance.onHealthChangedCallBack += UpdateHeartBar;
        InitializeHeartUI(); // Ensure heart UI is initialized on start
    }

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void InitializeHeartUI()
    {
        player = PlayerController.Instance;
        // Optionally find heartFill dynamically if it's not assigned or if it could change
        if (heartFill == null)
        {
            // Find the heartFill by tag, name, or however it's identifiable in your scene
            GameObject heartFillObject = GameObject.FindGameObjectWithTag("HeartFillTag");
            if (heartFillObject != null)
            {
                heartFill = heartFillObject.GetComponent<RectTransform>();
                if (heartFill != null)
                {
                    maxWidth = heartFill.sizeDelta.x; // Optionally reset maxWidth based on current UI setup
                }
            }
        }

        PlayerController.Instance.onHealthChangedCallBack += UpdateHeartBar;
        UpdateHeartBar(); // Update the heart bar immediately to reflect current health
    }


    public void UpdateHeartBar()
    {
        if (heartFill != null) // Check if heartFill is assigned
        {
            float healthPercentage = player.health / player.maxHealth;
            heartFill.sizeDelta = new Vector2(maxWidth * healthPercentage, heartFill.sizeDelta.y);
        }
        else
        {
            Debug.LogWarning("HeartFill RectTransform is not assigned.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeHeartUI(); // Re-initialize the heart UI whenever a new scene is loaded
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the sceneLoaded event to avoid memory leaks
    }


    void OnEnable()
    {
        // Subscribe to scene loaded if needed to reset health bar
    }

    void OnDisable()
    {
        // Unsubscribe to scene loaded if subscribed
    }

    // Add any other methods needed for scene transitions or additional logic
}
*/

/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartController : MonoBehaviour
{
    [SerializeField] private RectTransform heartFillRectTransform; // Optionally assign in the inspector
    [SerializeField] private string heartFillTag = "HeartFillTag"; // Use a tag to dynamically find the HeartFill if not assigned
    public float maxWidth; // The full width of the HeartFill at full health

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        InitializeHeartUI();
    }

private void InitializeHeartUI()
{
    if (heartFillRectTransform == null)
    {
        GameObject heartFillObject = GameObject.FindGameObjectWithTag("HeartFillTag"); // Replace "HeartFillTag" with your actual tag
        if (heartFillObject != null)
        {
                heartFillRectTransform = heartFillObject.GetComponent<RectTransform>();
            if (heartFillRectTransform != null)
            {
                Debug.Log("HeartFill found and assigned.");
                maxWidth = heartFillRectTransform.sizeDelta.x; // Optionally adjust this if needed
            }
            else
            {
                Debug.LogError("HeartFill object found, but missing RectTransform component.");
            }
        }
        else
        {
            Debug.LogError("Failed to find HeartFill object. Check your tags.");
        }
    }
}


    public void UpdateHeartBar(float healthPercentage)
    {
        if (heartFillRectTransform != null)
        {
            heartFillRectTransform.sizeDelta = new Vector2(maxWidth * healthPercentage, heartFillRectTransform.sizeDelta.y);
        }
        else
        {
            Debug.LogWarning("HeartFill RectTransform is not assigned.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeHeartUI(); // Re-initialize UI to ensure references are updated after scene load
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerController.Instance.onHealthChangedCallBack -= () => UpdateHeartBar(PlayerController.Instance.health / PlayerController.Instance.maxHealth);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
*/


/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartController : MonoBehaviour
{
    [SerializeField] private RectTransform heartFillRectTransform; // Assign in Inspector if possible
    [SerializeField] private string heartFillTag = "HeartFillTag"; // Ensure your heart fill GameObject is tagged correctly
    public float maxWidth; // Set dynamically based on the heartFill's initial size

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("HeartController Awake");
    }

    void Start()
    {
        InitializeHeartUI();
        Debug.Log("HeartController Start");
    }

    void InitializeHeartUI()
    {
        // Attempt to find the heartFill if not manually assigned
        if (heartFillRectTransform == null)
        {
            GameObject heartFillObject = GameObject.FindGameObjectWithTag(heartFillTag);
            if (heartFillObject)
            {
                heartFillRectTransform = heartFillObject.GetComponent<RectTransform>();
                if (heartFillRectTransform != null)
                {
                    maxWidth = heartFillRectTransform.sizeDelta.x; // Set maxWidth based on the RectTransform's current size
                    Debug.Log("HeartFill found and assigned dynamically.");
                }
                else
                {
                    Debug.LogError("HeartFill GameObject found, but it does not have a RectTransform component.");
                }
            }
            else
            {
                Debug.LogError("Failed to find HeartFill GameObject. Check if the tag is correctly applied.");
            }
        }
        else
        {
            Debug.Log("HeartFill assigned through Inspector.");
        }

        // Subscribe to health changes
        PlayerController.Instance.onHealthChangedCallBack += () => UpdateHeartBar(PlayerController.Instance.health / PlayerController.Instance.maxHealth);
    }

    public void UpdateHeartBar(float healthPercentage)
    {
        if (heartFillRectTransform != null)
        {
            heartFillRectTransform.sizeDelta = new Vector2(maxWidth * healthPercentage, heartFillRectTransform.sizeDelta.y);
            Debug.Log($"Updating heart bar to {healthPercentage * 100}%");
        }
        else
        {
            Debug.LogWarning("HeartFill RectTransform is not assigned.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}, reinitializing heart UI.");
        //InitializeHeartUI(); // Re-initialize UI to ensure references are updated after scene load
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.onHealthChangedCallBack -= () => UpdateHeartBar(PlayerController.Instance.health / PlayerController.Instance.maxHealth);
        }
    }
}
*/