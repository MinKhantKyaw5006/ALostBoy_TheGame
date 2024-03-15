using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatFollow : Enemy
{

    
    [SerializeField] private float lineOfSite;

    private Transform player;
    // Start is called before the first frame update
     void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        FollowingEnemy(); 
        base.Update(); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
    }

    private void FollowingEnemy()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer < lineOfSite)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, base.speed * Time.deltaTime);
        }

    }
}
