using System.Collections;
using UnityEngine;
using TMPro;

public class JumpPowerUp : MonoBehaviour
{
    public int additionalJumps = 2; // Number of additional jumps to allow
    public float duration = 5f; // How long the power-up effect lasts

    [Header("UI Components")]
    [SerializeField] private GameObject powerUpLogo;
    [SerializeField] private GameObject powerUpBackgroundPanel;
    [SerializeField] private TextMeshProUGUI powerUpCountdownText;
    [SerializeField] private TextMeshProUGUI powerUpAppliedText;

    private void Awake()
    {
        // Ensure the UI is hidden at the start
        powerUpLogo.SetActive(false);
        powerUpBackgroundPanel.SetActive(false);
        powerUpCountdownText.gameObject.SetActive(false);
        powerUpAppliedText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ApplyJumpBoost(collision));
        }
    }

    private IEnumerator ApplyJumpBoost(Collider2D player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.MaxAirJumps == 1) // Assuming the default is 1 air jump
        {
            powerUpAppliedText.text = "Jump PowerUp!";
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

            // Increase MaxAirJumps
            playerController.MaxAirJumps += additionalJumps;

            // Start the countdown
            float remainingDuration = duration;
            while (remainingDuration > 0)
            {
                powerUpCountdownText.text = $"{remainingDuration.ToString("0")}s";
                remainingDuration -= Time.deltaTime;
                yield return null;
            }

            // Revert MaxAirJumps back to original
            playerController.MaxAirJumps -= additionalJumps;



            // Destroy the power-up object
            Destroy(gameObject);
        }
    }
}
