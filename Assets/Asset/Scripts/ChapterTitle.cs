using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChapterTitle : MonoBehaviour
{
    [Header("Title Settings")]
    [SerializeField] private string[] titles; // Array of titles to show
    [SerializeField] private float fadeInDuration = 1.0f; // Duration to fade in
    [SerializeField] private float fadeOutDuration = 1.0f; // Duration to fade out
    [SerializeField] private float displayDuration = 2.0f; // Time the title is displayed before fading out
    [SerializeField] private float delayBetweenTitles = 0.5f; // Delay between showing titles

    [Header("TextMeshPro Settings")]
    [SerializeField] private GameObject titleObject; // The GameObject containing the TextMeshPro component

    private TextMeshProUGUI titleText; // Reference to the TextMeshPro component

    private void Awake()
    {
        titleText = titleObject.GetComponentInChildren<TextMeshProUGUI>();
        titleObject.SetActive(false); // Ensure the title is initially inactive
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ShowChapterTitles());
        }
    }

    private IEnumerator ShowChapterTitles()
    {
        titleObject.SetActive(true); // Activate the title object

        foreach (string title in titles)
        {
            yield return StartCoroutine(FadeIn(title));
            yield return new WaitForSeconds(displayDuration); // Wait while the title is displayed
            yield return StartCoroutine(FadeOut());
            yield return new WaitForSeconds(delayBetweenTitles); // Delay before the next title
        }

        titleObject.SetActive(false); // Deactivate the title object after displaying all titles
    }

    private IEnumerator FadeIn(string title)
    {
        titleText.text = title; // Set the text to be displayed
        Color textColor = titleText.color;
        textColor.a = 0; // Start transparent
        titleText.color = textColor;

        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(elapsedTime / fadeInDuration); // Fade in
            titleText.color = textColor;
            yield return null;
        }

        textColor.a = 1; // Ensure fully opaque
        titleText.color = textColor;
    }

    private IEnumerator FadeOut()
    {
        Color textColor = titleText.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(1 - (elapsedTime / fadeOutDuration)); // Fade out
            titleText.color = textColor;
            yield return null;
        }

        textColor.a = 0; // Ensure fully transparent
        titleText.color = textColor;
    }
}
