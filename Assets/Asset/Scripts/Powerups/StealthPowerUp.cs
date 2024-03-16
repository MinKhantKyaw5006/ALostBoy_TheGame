using System.Collections;
using UnityEngine;
using TMPro; // Make sure to include TextMeshPro namespace if using TextMeshPro for UI

public class StealthPowerUp : MonoBehaviour
{
    public float duration = 5f; // Duration of the stealth effect

    [Header("UI Components")]
    [SerializeField] private GameObject powerUpLogo;
    [SerializeField] private GameObject powerUpBackgroundPanel;
    [SerializeField] private TextMeshProUGUI powerUpCountdownText;
    [SerializeField] private TextMeshProUGUI powerUpAppliedText;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ApplyStealth(collision.gameObject));
        }
    }

    private IEnumerator ApplyStealth(GameObject player)
    {
        // Show UI with stealth message
        powerUpAppliedText.text = "Stealth Mode Activated!";

        // Hide the power-up object by disabling its collider and renderer
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = false;

        // Change player's tag to "Untagged"
        player.tag = "Untagged";

        // Show the power-up UI components for the countdown
        powerUpLogo.SetActive(true);
        powerUpBackgroundPanel.SetActive(true);
        powerUpCountdownText.gameObject.SetActive(true);

        powerUpAppliedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f); // Show the message for 2 seconds
        powerUpAppliedText.gameObject.SetActive(false);



        // Countdown for stealth duration
        float remainingDuration = duration;
        while (remainingDuration > 0)
        {
            powerUpCountdownText.text = $"{Mathf.Ceil(remainingDuration).ToString("0")}s";
            remainingDuration -= Time.deltaTime;
            yield return null;
        }

        // After stealth duration, revert player's tag and hide UI components
        player.tag = "Player";
      

        // Destroy or deactivate the power-up object
        Destroy(gameObject);
    }

}
