using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float zoomOutFactor = 1.5f; // Adjust this value as needed
    [SerializeField] private float zoomSpeed = 0.1f; // Speed of zooming
    private Vector3 originalOffset;
    private bool zoomedOut = false;

    void Start()
    {
        originalOffset = offset;
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnPlayerJump += HandlePlayerJump;
            PlayerController.Instance.OnPlayerLanded += HandlePlayerLanded;
        }
    }

    void OnDestroy()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnPlayerJump -= HandlePlayerJump;
            PlayerController.Instance.OnPlayerLanded -= HandlePlayerLanded;
        }
    }

    void FixedUpdate()
    {
        if (PlayerController.Instance != null)
        {
            Vector3 targetPosition = PlayerController.Instance.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);

            if (zoomedOut)
            {
                offset = Vector3.Lerp(offset, originalOffset, zoomSpeed);
                if (offset == originalOffset)
                {
                    zoomedOut = false;
                }
            }
        }
    }

    private void HandlePlayerJump(int jumpCount)
    {
        if (jumpCount > 1) // Zoom out on double jump or higher
        {
            offset = originalOffset * zoomOutFactor;
            zoomedOut = true;
        }
    }

    private void HandlePlayerLanded()
    {
        zoomedOut = false; // Reset zoomedOut flag when player lands
    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10); // Example offset
    [SerializeField] private float zoomSpeed = 0.05f; // Adjust for smoother transition
    private float currentZoomFactor = 1f; // Start with no zoom
    private float targetZoomFactor = 1f; // Target zoom level, changes based on jump
    private Vector3 originalOffset;

    void Start()
    {
        originalOffset = offset;
        // Attach delegates if using events from the PlayerController
    }

    void FixedUpdate()
    {
        HandleCameraFollowAndZoom();
    }

    private void HandleCameraFollowAndZoom()
    {
        if (playerTransform != null)
        {
            // Interpolate the current zoom factor towards the target zoom factor for smooth zooming
            currentZoomFactor = Mathf.Lerp(currentZoomFactor, targetZoomFactor, zoomSpeed);

            Vector3 adjustedOffset = originalOffset * currentZoomFactor;
            Vector3 targetPosition = playerTransform.position + adjustedOffset;

            // Smoothly interpolate the camera's position towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        }
    }

    private void HandlePlayerJump(int jumpCount)
    {
        // Adjust the target zoom factor based on the jump count
        targetZoomFactor = jumpCount == 1 ? firstJumpZoomFactor : secondJumpZoomFactor;
    }

    private void HandlePlayerLanded()
    {
        // Reset the zoom when the player lands
        targetZoomFactor = 1f;
    }

}

*/