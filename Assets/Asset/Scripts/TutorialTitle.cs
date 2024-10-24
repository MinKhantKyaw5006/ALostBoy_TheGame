using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialTitle : MonoBehaviour
{
    [System.Serializable]
    public class TitleSettings
    {
        public string titleText;              // Text to be displayed
        public float delayBeforeShow = 1f;    // Delay before the title is shown
        public float fadeInDuration = 1f;     // Fade in duration
        public float displayDuration = 2f;    // Time the title is fully visible
        public float fadeOutDuration = 1f;    // Fade out duration
    }

    [Header("Tutorial Title Settings")]
    [SerializeField] private TitleSettings[] titleSettingsArray; // Array of title settings

    [Header("TextMeshPro Settings")]
    [SerializeField] private GameObject titleObject; // The GameObject containing the TextMeshPro component

    private TextMeshProUGUI titleTextMesh; // Reference to the TextMeshPro component

    private void Awake()
    {
        titleTextMesh = titleObject.GetComponentInChildren<TextMeshProUGUI>();
        titleObject.SetActive(false); // Ensure the title is initially inactive
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(DisplayTutorialTitlesWithDelay());
        }
    }

    private IEnumerator DisplayTutorialTitlesWithDelay()
    {
        foreach (TitleSettings titleSetting in titleSettingsArray)
        {
            // Wait for the delay before showing the text
            yield return new WaitForSeconds(titleSetting.delayBeforeShow);

            // Set the title object active and start fade in/out process
            titleObject.SetActive(true);
            yield return StartCoroutine(FadeIn(titleSetting.titleText, titleSetting.fadeInDuration));
            yield return new WaitForSeconds(titleSetting.displayDuration); // Wait while the title is displayed
            yield return StartCoroutine(FadeOut(titleSetting.fadeOutDuration));
            titleObject.SetActive(false); // Hide after fade out
        }
    }

    private IEnumerator FadeIn(string text, float duration)
    {
        titleTextMesh.text = text; // Set the text to be displayed
        Color textColor = titleTextMesh.color;
        textColor.a = 0; // Start transparent
        titleTextMesh.color = textColor;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(elapsedTime / duration); // Fade in
            titleTextMesh.color = textColor;
            yield return null;
        }

        textColor.a = 1; // Ensure fully opaque
        titleTextMesh.color = textColor;
    }

    private IEnumerator FadeOut(float duration)
    {
        Color textColor = titleTextMesh.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(1 - (elapsedTime / duration)); // Fade out
            titleTextMesh.color = textColor;
            yield return null;
        }

        textColor.a = 0; // Ensure fully transparent
        titleTextMesh.color = textColor;
    }
}
