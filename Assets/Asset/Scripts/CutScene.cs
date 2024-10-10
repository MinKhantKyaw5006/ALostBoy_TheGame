using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    private bool isCutScenePlaying = false; // Track if the cutscene is playing

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playableDirector.Play();
            isCutScenePlaying = true; // Mark that the cutscene is playing
            GetComponent<BoxCollider2D>().enabled = false; // Disable the collider to prevent retriggering
        }
    }

    private void Update()
    {
        // Check if the cutscene is playing and if the space bar is pressed
        if (isCutScenePlaying && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene(); // Skip the cutscene if space bar is pressed
        }
    }

    private void SkipCutscene()
    {
        // Skip to the end of the timeline and evaluate
        playableDirector.time = playableDirector.duration;
        playableDirector.Evaluate(); // Evaluate to skip to the end properly

        OnCutsceneEnd(); // Call function to handle end of the cutscene
    }

    private void OnCutsceneEnd()
    {
        // Mark the cutscene as finished
        isCutScenePlaying = false;

        // Resume player control or handle any other necessary logic here
        // For example, if you disabled player movement during the cutscene, re-enable it here.

        Debug.Log("Cutscene ended, player can move again.");
    }
}
