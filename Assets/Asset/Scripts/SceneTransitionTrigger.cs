using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // The name of the scene you want to load

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Make sure your player GameObject has the "Player" tag
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
