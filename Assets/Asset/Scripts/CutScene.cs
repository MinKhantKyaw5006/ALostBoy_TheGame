using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

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

        // Show cutscene cover and play animation
        cutsceneCover.SetActive(true);
        StartCoroutine(SlideCoversIn());

        // Start the timeline
        playableDirector.Play();
        isCutScenePlaying = true;

        // Disable the collider to prevent retriggering
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update()
    {
        // Check if the cutscene is playing and if the space bar is pressed (to skip the cutscene)
        if (isCutScenePlaying && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }

        // Check if the cutscene has finished playing
        if (isCutScenePlaying && playableDirector.state != PlayState.Playing)
        {
            OnCutsceneEnd();
        }
    }

    private void SkipCutscene()
    {
        // Skip to the end of the timeline and evaluate
        playableDirector.time = playableDirector.duration;
        playableDirector.Evaluate(); // Evaluate to skip to the end properly

        OnCutsceneEnd(); // Call function to handle the end of the cutscene
    }

    private void OnCutsceneEnd()
    {
        // Mark the cutscene as finished
        isCutScenePlaying = false;

        // Show the UI elements again
        manaCover.SetActive(true);
        manaContainer.SetActive(true);
        collectableContainer.SetActive(true);
        heartContainer.SetActive(true);

        // Start sliding the black covers out
        StartCoroutine(SlideCoversOut());

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
