using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseControl : MonoBehaviour
{
    public FlyingEnemy[] enemyArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FlyingEnemy enemy in enemyArray)
            {
                enemy.Chase = true; // Use the property here
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FlyingEnemy enemy in enemyArray)
            {
                enemy.Chase = false; // Use the property here
            }
        }
    }
}