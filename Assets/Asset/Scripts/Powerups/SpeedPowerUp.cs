using System.Collections;
using UnityEngine;
using TMPro; // Include the TextMeshPro namespace

public class SpeedPowerUp : MonoBehaviour
{
    public float speedMultiplier = 2f; // The factor by which the speed is increased
    public float duration = 5f; // How long the power-up effect lasts

    [Header("UI Components")]
    [SerializeField] private GameObject powerUpLogo;
    [SerializeField] private GameObject powerUpBackgroundPanel;
    [SerializeField] private TextMeshProUGUI powerUpCountdownText; // For the countdown timer
    [SerializeField] private TextMeshProUGUI powerUpAppliedText; // For the "Agility PowerUp!" message

    private void Awake()
    {
        // Optionally, make sure the UI is hidden at the start
        powerUpLogo.SetActive(false);
        powerUpBackgroundPanel.SetActive(false);
        powerUpCountdownText.gameObject.SetActive(false);
        powerUpAppliedText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ApplySpeedBoost(collision));
        }
    }

    private IEnumerator ApplySpeedBoost(Collider2D player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && !playerController.speedBoostActive)
        {
            // Show the "Agility PowerUp!" message briefly
            powerUpAppliedText.text = "Agility PowerUp!";
            // Hide the power-up object by disabling its collider and renderer
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = false;

            // Show the power-up UI components for the countdown
            powerUpLogo.SetActive(true);
            powerUpBackgroundPanel.SetActive(true);
            powerUpCountdownText.gameObject.SetActive(true);

            powerUpAppliedText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f); // Show the message for 2 seconds
            powerUpAppliedText.gameObject.SetActive(false);



            // Activate speed boost flag
            //playerController.speedBoostActive = true;

            // Store the original speed
            float originalSpeed = playerController.walkspeed;

            // Apply speed boost
            playerController.walkspeed *= speedMultiplier;

           

            // Start the countdown
            float remainingDuration = duration;
            while (remainingDuration > 0)
            {
                powerUpCountdownText.text = $"{remainingDuration.ToString("0")}s";
                remainingDuration -= Time.deltaTime;
                yield return null; // Wait for a frame
            }

            // Revert player's speed back to normal
            playerController.walkspeed = originalSpeed;

            // Deactivate speed boost flag and hide the UI components
            playerController.speedBoostActive = false;
            powerUpLogo.SetActive(false);
            powerUpBackgroundPanel.SetActive(false);
            powerUpCountdownText.gameObject.SetActive(false);

            // Destroy the power-up object
            Destroy(gameObject);
        }
    }
}
