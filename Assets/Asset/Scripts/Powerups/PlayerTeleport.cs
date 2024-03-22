using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // For TextMeshPro elements
using UnityEngine.UI; // For built-in UI Text elements


public class PlayerTeleport : MonoBehaviour
{
    private GameObject currentTeleporter;
    private PlayerControls playerControls;
    public TextMeshProUGUI teleportText; // If using TextMeshPro
                                  //public Text teleportText; // If using Unity's built-in UI system


    private void Awake()
    {
        // Initialize the PlayerControls
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        playerControls.Player.Attack.performed -= OnAttackPerformed;
        playerControls.Disable();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // Check if there's a current teleporter to use
        if (currentTeleporter != null)
        {
            Transform destination = currentTeleporter.GetComponent<Teleporter>().GetDestination();
            if (destination != null)
            {
                transform.position = destination.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
            teleportText.gameObject.SetActive(true); // Show the text
            // Play the teleport sound effect
            Teleporter teleporter = currentTeleporter.GetComponent<Teleporter>();
            if (teleporter != null)
            {
                teleporter.PlayTeleportSound();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter") && collision.gameObject == currentTeleporter)
        {
            teleportText.gameObject.SetActive(false); // Hide the text
            currentTeleporter = null;
        }
    }
}
