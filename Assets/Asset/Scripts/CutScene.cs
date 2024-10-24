//using System.Collections;
//using UnityEngine;
//using UnityEngine.Playables;
//using UnityEngine.UI;

//public class CutScene : MonoBehaviour
//{
//    [SerializeField] private PlayableDirector playableDirector;
//    private bool isCutScenePlaying = false; // Track if the cutscene is playing

//    // UI Elements to hide during cutscene
//    public GameObject manaCover;
//    public GameObject manaContainer;
//    public GameObject collectableContainer;
//    public GameObject heartContainer;

//    // Cutscene black cover
//    public GameObject cutsceneCover;  // Parent object for upper and lower covers
//    public RectTransform upperCover;  // The top sliding rectangle
//    public RectTransform lowerCover;  // The bottom sliding rectangle

//    [SerializeField] private float animationDuration = 1.0f;  // Time taken for the covers to slide in

//    // Positions for the upper and lower covers
//    [SerializeField] private Vector2 upperInitialPos = new Vector2(0, 425f);  // Initial position for the upper cover
//    [SerializeField] private Vector2 lowerInitialPos = new Vector2(0, -425f); // Initial position for the lower cover

//    [SerializeField] private Vector2 upperTargetPos = new Vector2(0, 300f);  // Target position for the upper cover during cutscene
//    [SerializeField] private Vector2 lowerTargetPos = new Vector2(0, -300f); // Target position for the lower cover during cutscene

//    private PlayerControls playerControls;

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            StartCutscene(); // Start cutscene logic
//        }
//    }
//    private void Awake()
//    {
//        playerControls = new PlayerControls();
//        playerControls.Enable(); // Enable the controls
//    }



//    private void StartCutscene()
//    {
//        // Hide UI elements
//        manaCover.SetActive(false);
//        manaContainer.SetActive(false);
//        collectableContainer.SetActive(false);
//        heartContainer.SetActive(false);

//        // Show cutscene cover and play animation
//        cutsceneCover.SetActive(true);
//        StartCoroutine(SlideCoversIn());

//        // Start the timeline
//        playableDirector.Play();
//        isCutScenePlaying = true;

//        // Disable the collider to prevent retriggering
//        GetComponent<BoxCollider2D>().enabled = false;
//    }

//    private void Update()
//    {
//        // Check if the cutscene is playing and if the space bar is pressed (to skip the cutscene)
//        if (isCutScenePlaying && Input.GetKeyDown(KeyCode.Space))
//        {
//            SkipCutscene();
//        }

//        // Check if the cutscene has finished playing
//        if (isCutScenePlaying && playableDirector.state != PlayState.Playing)
//        {
//            OnCutsceneEnd();
//        }
//    }

//    private void SkipCutscene()
//    {
//        // Skip to the end of the timeline and evaluate
//        playableDirector.time = playableDirector.duration;
//        playableDirector.Evaluate(); // Evaluate to skip to the end properly

//        OnCutsceneEnd(); // Call function to handle the end of the cutscene
//    }

//    private void OnCutsceneEnd()
//    {
//        // Mark the cutscene as finished
//        isCutScenePlaying = false;

//        // Show the UI elements again
//        manaCover.SetActive(true);
//        manaContainer.SetActive(true);
//        collectableContainer.SetActive(true);
//        heartContainer.SetActive(true);

//        // Start sliding the black covers out
//        StartCoroutine(SlideCoversOut());

//        Debug.Log("Cutscene ended, player can move again.");
//    }

//    // Coroutine to slide the black covers in (move to 300 and -300)
//    private IEnumerator SlideCoversIn()
//    {
//        Vector2 initialUpperPos = upperCover.anchoredPosition;
//        Vector2 initialLowerPos = lowerCover.anchoredPosition;

//        float elapsedTime = 0f;

//        while (elapsedTime < animationDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float t = elapsedTime / animationDuration;

//            // Move covers
//            upperCover.anchoredPosition = Vector2.Lerp(initialUpperPos, upperTargetPos, t);
//            lowerCover.anchoredPosition = Vector2.Lerp(initialLowerPos, lowerTargetPos, t);

//            yield return null;
//        }

//        // Ensure final positions are set
//        upperCover.anchoredPosition = upperTargetPos;
//        lowerCover.anchoredPosition = lowerTargetPos;
//    }

//    // Coroutine to slide the black covers out (back to 425 and -425)
//    private IEnumerator SlideCoversOut()
//    {
//        Vector2 initialUpperPos = upperCover.anchoredPosition;
//        Vector2 initialLowerPos = lowerCover.anchoredPosition;

//        float elapsedTime = 0f;

//        while (elapsedTime < animationDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float t = elapsedTime / animationDuration;

//            // Move covers back to their original positions
//            upperCover.anchoredPosition = Vector2.Lerp(initialUpperPos, upperInitialPos, t);
//            lowerCover.anchoredPosition = Vector2.Lerp(initialLowerPos, lowerInitialPos, t);

//            yield return null;
//        }

//        // Hide cutscene cover after the animation
//        cutsceneCover.SetActive(false);
//    }
//}


//working code
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Playables;
//using UnityEngine.InputSystem;
//using TMPro; // Include TextMeshPro namespace

//public class CutScene : MonoBehaviour
//{
//    [SerializeField] private PlayableDirector playableDirector;
//    private bool isCutScenePlaying = false; // Track if the cutscene is playing

//    // UI Elements to hide during cutscene
//    public GameObject manaCover;
//    public GameObject manaContainer;
//    public GameObject collectableContainer;
//    public GameObject heartContainer;

//    // Cutscene black cover
//    public GameObject cutsceneCover;  // Parent object for upper and lower covers
//    public RectTransform upperCover;  // The top sliding rectangle
//    public RectTransform lowerCover;  // The bottom sliding rectangle

//    [SerializeField] private float animationDuration = 1.0f;  // Time taken for the covers to slide in

//    // Positions for the upper and lower covers
//    [SerializeField] private Vector2 upperInitialPos = new Vector2(0, 425f);  // Initial position for the upper cover
//    [SerializeField] private Vector2 lowerInitialPos = new Vector2(0, -425f); // Initial position for the lower cover

//    [SerializeField] private Vector2 upperTargetPos = new Vector2(0, 300f);  // Target position for the upper cover during cutscene
//    [SerializeField] private Vector2 lowerTargetPos = new Vector2(0, -300f); // Target position for the lower cover during cutscene

//    private PlayerControls playerControls;

//    // Reference to the skip message UI
//    public GameObject skipMessage; // Assign this in the Inspector
//    public bool showSkipMessage = false; // Control to show/hide the skip message
//    private bool isSkipMessageActive = false; // Track if the skip message is active

//    // Control whether to show the black covers during the cutscene
//    public bool showBlackCovers = true; // Control to show/hide the black covers

//    private void Awake()
//    {
//        playerControls = new PlayerControls();
//    }

//    private void OnEnable()
//    {
//        // Subscribe to Jump action for skipping cutscene
//        playerControls.Player.Jump.performed += OnJumpPerformed;
//        playerControls.Enable();
//    }

//    private void OnDisable()
//    {
//        // Unsubscribe from the actions
//        playerControls.Player.Jump.performed -= OnJumpPerformed;
//        playerControls.Disable();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            StartCutscene(); // Start cutscene logic
//        }
//    }

//    private void StartCutscene()
//    {
//        // Hide UI elements
//        manaCover.SetActive(false);
//        manaContainer.SetActive(false);
//        collectableContainer.SetActive(false);
//        heartContainer.SetActive(false);

//        // Show cutscene cover and play animation if enabled
//        if (showBlackCovers)
//        {
//            cutsceneCover.SetActive(true);
//            StartCoroutine(SlideCoversIn());
//        }

//        // Start the timeline
//        playableDirector.Play();
//        isCutScenePlaying = true;

//        // Show the skip message if enabled in the inspector
//        if (showSkipMessage)
//        {
//            skipMessage.SetActive(true); // Activate the skip message
//            isSkipMessageActive = true; // Mark it as active
//        }

//        // Disable the collider to prevent retriggering
//        GetComponent<BoxCollider2D>().enabled = false;
//    }

//    private void Update()
//    {
//        // Check if the cutscene has finished playing
//        if (isCutScenePlaying && playableDirector.state != PlayState.Playing)
//        {
//            OnCutsceneEnd();
//        }
//    }

//    private void OnJumpPerformed(InputAction.CallbackContext context)
//    {
//        // Skip the cutscene when the Jump action is performed, only if showSkipMessage is true
//        if (isCutScenePlaying && showSkipMessage)
//        {
//            SkipCutscene();
//        }
//    }

//    private void SkipCutscene()
//    {
//        // Skip to the end of the timeline and evaluate
//        playableDirector.time = playableDirector.duration;
//        playableDirector.Evaluate(); // Evaluate to skip to the end properly

//        OnCutsceneEnd(); // Call function to handle the end of the cutscene
//    }

//    private void OnCutsceneEnd()
//    {
//        // Mark the cutscene as finished
//        isCutScenePlaying = false;

//        // Hide the skip message if it was active
//        if (isSkipMessageActive)
//        {
//            skipMessage.SetActive(false); // Hide the skip message
//            isSkipMessageActive = false; // Reset the skip message status
//        }

//        // Show the UI elements again
//        manaCover.SetActive(true);
//        manaContainer.SetActive(true);
//        collectableContainer.SetActive(true);
//        heartContainer.SetActive(true);

//        // Start sliding the black covers out if they were shown
//        if (showBlackCovers)
//        {
//            StartCoroutine(SlideCoversOut());
//        }

//        Debug.Log("Cutscene ended, player can move again.");
//    }

//    // Coroutine to slide the black covers in (move to 300 and -300)
//    private IEnumerator SlideCoversIn()
//    {
//        Vector2 initialUpperPos = upperCover.anchoredPosition;
//        Vector2 initialLowerPos = lowerCover.anchoredPosition;

//        float elapsedTime = 0f;

//        while (elapsedTime < animationDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float t = elapsedTime / animationDuration;

//            // Move covers
//            upperCover.anchoredPosition = Vector2.Lerp(initialUpperPos, upperTargetPos, t);
//            lowerCover.anchoredPosition = Vector2.Lerp(initialLowerPos, lowerTargetPos, t);

//            yield return null;
//        }

//        // Ensure final positions are set
//        upperCover.anchoredPosition = upperTargetPos;
//        lowerCover.anchoredPosition = lowerTargetPos;
//    }

//    // Coroutine to slide the black covers out (back to 425 and -425)
//    private IEnumerator SlideCoversOut()
//    {
//        Vector2 initialUpperPos = upperCover.anchoredPosition;
//        Vector2 initialLowerPos = lowerCover.anchoredPosition;

//        float elapsedTime = 0f;

//        while (elapsedTime < animationDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float t = elapsedTime / animationDuration;

//            // Move covers back to their original positions
//            upperCover.anchoredPosition = Vector2.Lerp(initialUpperPos, upperInitialPos, t);
//            lowerCover.anchoredPosition = Vector2.Lerp(initialLowerPos, lowerInitialPos, t);

//            yield return null;
//        }

//        // Hide cutscene cover after the animation
//        cutsceneCover.SetActive(false);
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using TMPro; // Include TextMeshPro namespace

public class CutScene : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    private bool isCutScenePlaying = false; // Track if the cutscene is playing

    // UI Elements to hide during cutscene
    public GameObject manaCover;
    public GameObject manaContainer;
    public GameObject collectableContainer;
    public GameObject heartContainer;

    // Cutscene black cover
    public GameObject cutsceneCover;  // Parent object for upper and lower covers
    public RectTransform upperCover;  // The top sliding rectangle
    public RectTransform lowerCover;  // The bottom sliding rectangle

    [SerializeField] private float animationDuration = 1.0f;  // Time taken for the covers to slide in

    // Positions for the upper and lower covers
    [SerializeField] private Vector2 upperInitialPos = new Vector2(0, 425f);  // Initial position for the upper cover
    [SerializeField] private Vector2 lowerInitialPos = new Vector2(0, -425f); // Initial position for the lower cover

    [SerializeField] private Vector2 upperTargetPos = new Vector2(0, 300f);  // Target position for the upper cover during cutscene
    [SerializeField] private Vector2 lowerTargetPos = new Vector2(0, -300f); // Target position for the lower cover during cutscene

    private PlayerControls playerControls;

    // Reference to the skip message UI
    public GameObject skipMessage; // Assign this in the Inspector
    public bool showSkipMessage = false; // Control to show/hide the skip message
    private bool isSkipMessageActive = false; // Track if the skip message is active

    // Control whether to show the black covers during the cutscene
    public bool showBlackCovers = true; // Control to show/hide the black covers

    // Variable to hold the unique skip message
    [TextArea(3, 10)] // Optional attribute for multiline text
    public string skipMessageText; // Assign this in the Inspector

    // Delay time for showing the skip message
    [SerializeField] private float skipMessageDelay = 0f; // Delay in seconds

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        // Subscribe to Jump action for skipping cutscene
        playerControls.Player.Jump.performed += OnJumpPerformed;
        playerControls.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions
        playerControls.Player.Jump.performed -= OnJumpPerformed;
        playerControls.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCutscene(); // Start cutscene logic
        }
    }

    private void StartCutscene()
    {
        // Hide UI elements
        manaCover.SetActive(false);
        manaContainer.SetActive(false);
        collectableContainer.SetActive(false);
        heartContainer.SetActive(false);

        // Show cutscene cover and play animation if enabled
        if (showBlackCovers)
        {
            cutsceneCover.SetActive(true);
            StartCoroutine(SlideCoversIn());
        }

        // Start the timeline
        playableDirector.Play();
        isCutScenePlaying = true;

        // Show the skip message if enabled in the inspector
        if (showSkipMessage)
        {
            StartCoroutine(ShowSkipMessageAfterDelay());
        }

        // Disable the collider to prevent retriggering
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private IEnumerator ShowSkipMessageAfterDelay()
    {
        // Wait for the specified delay before showing the skip message
        yield return new WaitForSeconds(skipMessageDelay);

        // Activate the skip message
        skipMessage.SetActive(true);
        isSkipMessageActive = true; // Mark it as active
        // Update the text of the skip message with the specified text
        skipMessage.GetComponentInChildren<TextMeshProUGUI>().text = skipMessageText; // Set the text from the inspector
    }

    private void Update()
    {
        // Check if the cutscene has finished playing
        if (isCutScenePlaying && playableDirector.state != PlayState.Playing)
        {
            OnCutsceneEnd();
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // Skip the cutscene when the Jump action is performed, only if showSkipMessage is true
        if (isCutScenePlaying && showSkipMessage)
        {
            SkipCutscene();
        }
    }

    private void SkipCutscene()
    {
        // Skip to the end of the timeline and evaluate
        playableDirector.time = playableDirector.duration;
        playableDirector.Evaluate(); // Evaluate to skip to the end properly

        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null && DialogueManager.isActive)
        {
            //dialogueManager.StopDialogue(); // Stop the dialogue if active
        }

        OnCutsceneEnd(); // Call function to handle the end of the cutscene
    }

    private void OnCutsceneEnd()
    {
        // Mark the cutscene as finished
        isCutScenePlaying = false;

        // Hide the skip message if it was active
        if (isSkipMessageActive)
        {
            skipMessage.SetActive(false); // Hide the skip message
            isSkipMessageActive = false; // Reset the skip message status
        }

        // Show the UI elements again
        manaCover.SetActive(true);
        manaContainer.SetActive(true);
        collectableContainer.SetActive(true);
        heartContainer.SetActive(true);

        // Start sliding the black covers out if they were shown
        if (showBlackCovers)
        {
            StartCoroutine(SlideCoversOut());
        }

        // Stop the dialogue if active when the cutscene ends
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null && DialogueManager.isActive)
        {
           // dialogueManager.StopDialogue();
        }

        Debug.Log("Cutscene ended, player can move again.");
    }

    // Coroutine to slide the black covers in (move to 300 and -300)
    private IEnumerator SlideCoversIn()
    {
        Vector2 initialUpperPos = upperCover.anchoredPosition;
        Vector2 initialLowerPos = lowerCover.anchoredPosition;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Move covers
            upperCover.anchoredPosition = Vector2.Lerp(initialUpperPos, upperTargetPos, t);
            lowerCover.anchoredPosition = Vector2.Lerp(initialLowerPos, lowerTargetPos, t);

            yield return null;
        }

        // Ensure final positions are set
        upperCover.anchoredPosition = upperTargetPos;
        lowerCover.anchoredPosition = lowerTargetPos;
    }

    // Coroutine to slide the black covers out (back to 425 and -425)
    private IEnumerator SlideCoversOut()
    {
        Vector2 initialUpperPos = upperCover.anchoredPosition;
        Vector2 initialLowerPos = lowerCover.anchoredPosition;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Move covers back to their original positions
            upperCover.anchoredPosition = Vector2.Lerp(initialUpperPos, upperInitialPos, t);
            lowerCover.anchoredPosition = Vector2.Lerp(initialLowerPos, lowerInitialPos, t);

            yield return null;
        }

        // Hide cutscene cover after the animation
        cutsceneCover.SetActive(false);
    }
}
