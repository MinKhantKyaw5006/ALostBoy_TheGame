using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        if (PlayerController.Instance != null)
        {
            Vector3 targetPosition = PlayerController.Instance.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        }
    }
}
