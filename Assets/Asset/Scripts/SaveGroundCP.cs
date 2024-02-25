using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGroundCP : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCheckPoint;

    public Vector2 safeGroundLocation { get; private set; } = Vector2.zero;

    private void Start()
    {
        //initializing   a starting safe position
        safeGroundLocation = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the collider gameobject is within the whatischeckpoint layermask
        if((whatIsCheckPoint.value &(1 << collision.gameObject.layer)) > 0)
        {
            //update the safegroundlocation
            safeGroundLocation = new Vector2(collision.bounds.center.x, collision.bounds.min.y);
        }

    }

    public void WarpPlayerToSafeGround()
    {
        transform.position = safeGroundLocation;
    }
}
