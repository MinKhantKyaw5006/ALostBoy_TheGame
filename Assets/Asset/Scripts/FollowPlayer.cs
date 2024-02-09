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
