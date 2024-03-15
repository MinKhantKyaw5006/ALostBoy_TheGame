using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabObjects : MonoBehaviour
{
    [SerializeField] private Transform grabPoint;
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDistance;
    private Vector3 offset;

    private GameObject grabbedObject;
    private int layerIndex;

    private void Start()
    {
        layerIndex = LayerMask.NameToLayer("Objects");
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);

        if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex)
        {
            // Grab object
            if (Keyboard.current.rKey.wasPressedThisFrame && grabbedObject == null)
            {
                grabbedObject = hitInfo.collider.gameObject;
                Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
                rb.isKinematic = true;
                offset = grabbedObject.transform.position - grabPoint.position; // Calculate offset
                grabbedObject.transform.SetParent(transform);
            }

            // Release object
            else if (Keyboard.current.rKey.wasPressedThisFrame && grabbedObject != null)
            {
                Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
                rb.isKinematic = false;
                grabbedObject.transform.SetParent(null);
                grabbedObject = null;
            }
        }

        Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.red);
    }

    private void FixedUpdate()
    {
        if (grabbedObject != null)
        {
            // Update grabbed object's position with offset
            grabbedObject.transform.position = grabPoint.position + offset;
        }
    }
}
